using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.UI.Contract;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Audio;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Systems;
using Domivium.Client.Core.UI.Contract;
using Domivium.Client.Core.UI.Navigation;
using Domivium.Client.Core.UI.Presenter;
using Domivium.Client.Core.Utility;
using Domivium.Client.Data.Item;
using Domivium.Client.Data.Stat;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Domivium.Client.Contents.UI.Stack
{
    public class ItemContainerStackUIPresenter : StackUIPresenter<ItemContainerStackUIView, IItemContainerStackUIMessage>, IItemContainerStackUIMessage
    {
        private readonly IItemSystem _itemSystem;
        private readonly ISpriteSystem _spriteSystem;
        private readonly ICharacterSystemCommand _characterSystemCommand;
        private readonly IItemSystemCommand _itemSystemCommand;

        private ItemSlotEntry _selectedSlot = ItemSlotEntry.Default;
        private ItemSlotType _openType = ItemSlotType.None;
        private IBattleSystem _character;

        public ItemContainerStackUIPresenter(
            ItemContainerStackUIView view,
            IUINavigation navigation,
            IAudioPlayer audioController,
            IActorManager actorManager,
            ICameraSystem cameraSystem,
            ISpriteSystem spriteSystem,
            IItemSystem itemSystem,
            ICharacterSystemCommand characterSystemCommand,
            IItemSystemCommand itemSystemCommand)
            : base(view, navigation, audioController)
        {
            actorManager.Character.Subscribe(OnChangeCharacter).AddTo(ref DisposableBag);
            view.SetUICamera(cameraSystem.UICamera);

            _spriteSystem = spriteSystem;
            _itemSystem = itemSystem;
            _itemSystem.FilledInventoryCapacity.Subscribe(View.SetFilledInventoryCapacity).AddTo(ref DisposableBag);
            _itemSystem.InventoryCapacity.Subscribe(View.SetInventoryCapacity).AddTo(ref DisposableBag);
            _itemSystem.FilledLootCapacity.Subscribe(View.SetFilledLootCapacity).AddTo(ref DisposableBag);
            _itemSystem.LootCapacity.Subscribe(View.SetLootCapacity).AddTo(ref DisposableBag);
            _itemSystem.Equipment.CollectionChanged += OnChangedEquipment;
            _itemSystem.Inventory.CollectionChanged += OnChangedInventory;
            _itemSystem.Loot.CollectionChanged += OnChangedLoot;

            _characterSystemCommand = characterSystemCommand;
            _itemSystemCommand = itemSystemCommand;
        }

        protected override void OnDispose()
        {
            _itemSystem.Equipment.CollectionChanged -= OnChangedEquipment;
            _itemSystem.Inventory.CollectionChanged -= OnChangedInventory;
            _itemSystem.Loot.CollectionChanged -= OnChangedLoot;
            base.OnDispose();
        }

        public override async UniTask ShowAsync(CancellationToken token, UIParam param, bool immediately = false)
        {
            await base.ShowAsync(token, param, immediately);

            _openType = param switch
            {
                InventoryParams => ItemSlotType.Inventory,
                InventoryWithLootParams => ItemSlotType.Loot,
                _ => _openType
            };


            foreach (var (index, item) in _itemSystem.Equipment)
            {
                SetItem(ItemSlotType.Equipment, index, item);
            }

            foreach (var (index, item) in _itemSystem.Inventory)
            {
                SetItem(ItemSlotType.Inventory, index, item);
            }

            View.SetInventoryCapacity(_itemSystem.InventoryCapacity.CurrentValue);
            View.SetLootCapacity(_itemSystem.LootCapacity.CurrentValue);
            View.SetMoney(0); //temp
            View.SetGem(0); //temp
            View.SetLevel(1); //temp
            View.SetExp(0, 100); //temp
            OnHealthGaugeChanged();
            OnHungerGaugeChanged();
            OnStaminaGaugeChanged();
            OnSanityGaugeChanged();
            OnWeightGaugeChanged();
        }

        public override void OnHideExit()
        {
            base.OnHideExit();
            _characterSystemCommand.Stop();
        }

        public void OnClick(ItemSlotEntry slot)
        {
            if (_itemSystem.TryGetItem(slot, out var item) && !_selectedSlot.IsSame(slot))
            {
                var context = _itemSystem.GetItemContext(item.Type, item.Id);
                View.SelectItem(slot, item, context);
                _selectedSlot = slot;
            }
            else
            {
                View.DeselectItem();
                _selectedSlot = ItemSlotEntry.Default;
            }
        }

        public void OnBeginDrag(ItemSlotEntry slot, Vector2 position)
        {
            if (!_itemSystem.TryGetItem(slot, out var item)) return;

            View.PickItem(slot, item.Type, position);
            _selectedSlot = slot;
        }

        public void OnDrag(Vector2 position)
        {
            View.MoveItem(position);
        }

        public void OnEndDrag(ItemSlotEntry sourceSlot, Vector2 position)
        {
            View.DropItem();

            if (!View.TryGetSlot(position, out var targetSlot)) return;

            switch (sourceSlot.Type)
            {
                case ItemSlotType.Equipment:
                    switch (targetSlot.Type)
                    {
                        case ItemSlotType.Equipment:
                        case ItemSlotType.Loot:
                            return;
                        case ItemSlotType.Inventory:
                            _itemSystemCommand.Unequip(sourceSlot, targetSlot);
                            return;
                        case ItemSlotType.None:
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case ItemSlotType.Inventory:
                    switch (targetSlot.Type)
                    {
                        case ItemSlotType.Equipment:
                            _itemSystemCommand.Equip(sourceSlot, targetSlot);
                            return;
                        case ItemSlotType.Inventory:
                        case ItemSlotType.Loot:
                            _itemSystemCommand.SwapOrMerge(sourceSlot, targetSlot);
                            return;
                        case ItemSlotType.None:
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case ItemSlotType.Loot:
                    switch (targetSlot.Type)
                    {
                        case ItemSlotType.Equipment:
                            return;
                        case ItemSlotType.Inventory:
                        case ItemSlotType.Loot:
                            _itemSystemCommand.SwapOrMerge(sourceSlot, targetSlot);
                            return;
                        case ItemSlotType.None:
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case ItemSlotType.None:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void OnEquip(ItemSlotEntry slot)
        {
            View.DeselectItem();

            if (!_itemSystem.TryGetItem(slot, out var item)) return;

            var targetSlot = new ItemSlotEntry(ItemSlotType.Equipment, Converter.GetSlotIndex(item.Type));
            _itemSystemCommand.Equip(slot, targetSlot);
        }

        public void OnUnequip(ItemSlotEntry slot)
        {
            View.DeselectItem();

            if (!_itemSystem.TryGetItem(slot, out _)) return;

            if (!_itemSystem.TryGetEmptySlotIndex(ItemSlotType.Inventory, out var slotIndex)) return;

            var targetSlot = new ItemSlotEntry(ItemSlotType.Inventory, slotIndex);
            _itemSystemCommand.Unequip(slot, targetSlot);
        }

        public void OnShowSplitter(ItemSlotEntry slot)
        {
            if (!_itemSystem.TryGetItem(slot, out var item)) return;

            View.ShowSplitter(slot, item.Count);
        }

        public void OnUse(ItemSlotEntry slot)
        {
            View.DeselectItem();
        }

        public void OnKeep(ItemSlotEntry slot)
        {
            View.DeselectItem();

            if (!_itemSystem.TryGetEmptySlotIndex(_openType, out var slotIndex)) return;

            _itemSystemCommand.SwapOrMerge(slot, new ItemSlotEntry(_openType, slotIndex));
        }

        public void OnTakeOut(ItemSlotEntry slot)
        {
            View.DeselectItem();

            if (!_itemSystem.TryGetEmptySlotIndex(ItemSlotType.Inventory, out var slotIndex)) return;

            _itemSystemCommand.SwapOrMerge(slot, new ItemSlotEntry(ItemSlotType.Inventory, slotIndex));
        }

        public void OnRemove(ItemSlotEntry slot)
        {
            View.DeselectItem();

            if (!_itemSystem.TryGetItem(slot, out _)) return;

            _itemSystemCommand.Remove(slot);
        }

        public void OnSplit(ItemSlotEntry slot, int count)
        {
            View.DeselectItem();
            _itemSystemCommand.SplitStack(slot, count);
        }

        private void SetItem(ItemSlotType slotType, int index, ItemEntity item)
        {
            var sprite = _spriteSystem.GetSprite(item.Type, item.Id);
            var itemCount = item.Count;
            var isStackable = item.IsStackable;
            switch (slotType)
            {
                case ItemSlotType.Equipment:
                    View.SetEquipmentSlot(index, sprite, itemCount, isStackable);
                    break;
                case ItemSlotType.Inventory:
                    View.SetInventorySlot(index, sprite, itemCount, isStackable);
                    break;
                case ItemSlotType.Loot:
                    View.SetLootSlot(index, sprite, itemCount, isStackable);
                    break;
                case ItemSlotType.None:
                default:
                    throw new ArgumentOutOfRangeException(nameof(slotType), slotType, null);
            }
        }

        private void ClearItem(ItemSlotType slotType, int index)
        {
            switch (slotType)
            {
                case ItemSlotType.Equipment:
                    View.ClearEquipmentSlot(index);
                    break;
                case ItemSlotType.Inventory:
                    View.ClearInventorySlot(index);
                    break;
                case ItemSlotType.Loot:
                    View.ClearLootSlot(index);
                    break;
                case ItemSlotType.None:
                default:
                    throw new ArgumentOutOfRangeException(nameof(slotType), slotType, null);
            }
        }

        private void SetGauge(StatId statId)
        {
            var current = _character.Gauge.Current(statId);
            var limit = _character.Stat.Value(statId);
            View.SetGauge(statId, current, limit);
        }

        private void SetStat(StatId statId)
        {
            if (StatSet.GetDomain(statId) == StatDomain.Value)
            {
                View.SetStat(statId, _character.Stat.Value(statId));
            }
            else
            {
                View.SetStat(statId, _character.Stat.RateValue(statId));
            }
        }

        private void SetWeight()
        {
            var current = _character.Gauge.Current(StatId.Weight);
            var limit = _character.Stat.Value(StatId.Weight);
            View.SetWeight(current, limit);
        }

        private void OnHealthStatChanged() => SetGauge(StatId.Health);

        private void OnHungerStatChanged() => SetGauge(StatId.Hunger);

        private void OnStaminaStatChanged() => SetGauge(StatId.Stamina);

        private void OnSanityStatChanged() => SetGauge(StatId.Hunger);
        private void OnDurabilityStatChanged() { }
        private void OnInventoryCapacityStatChanged() { }
        private void OnProjectileCapacityStatChanged() => SetStat(StatId.ProjectileCapacity);
        private void OnAttackStatChanged() => SetStat(StatId.Attack);
        private void OnDefenseStatChanged() => SetStat(StatId.Defense);
        private void OnAttackRangeStatChanged() => SetStat(StatId.AttackRange);
        private void OnMoveSpeedStatChanged() => SetStat(StatId.MoveSpeed);
        private void OnAttackSpeedStatChanged() => SetStat(StatId.AttackSpeed);
        private void OnReloadSpeedStatChanged() => SetStat(StatId.ReloadSpeed);
        private void OnProjectileSpeedStatChanged() => SetStat(StatId.ProjectileSpeed);
        private void OnCriticalRateStatChanged() => SetStat(StatId.CriticalRate);
        private void OnCriticalDamageStatChanged() => SetStat(StatId.CriticalDamage);
        private void OnWeightStatChanged() => SetWeight();

        private void OnHealthGaugeChanged() => SetGauge(StatId.Health);

        private void OnHungerGaugeChanged() => SetGauge(StatId.Hunger);

        private void OnStaminaGaugeChanged() => SetGauge(StatId.Stamina);

        private void OnSanityGaugeChanged() => SetGauge(StatId.Sanity);

        private void OnWeightGaugeChanged() => SetWeight();

        private void OnChangeCharacter(IUnitPresenter character)
        {
            if (character == null) return;

            _character = character.BattleSystem;
            _character.Stat.AddListener(StatId.Health, OnHealthStatChanged);
            _character.Stat.AddListener(StatId.Hunger, OnHungerStatChanged);
            _character.Stat.AddListener(StatId.Stamina, OnStaminaStatChanged);
            _character.Stat.AddListener(StatId.Sanity, OnSanityStatChanged);
            _character.Stat.AddListener(StatId.Durability, OnDurabilityStatChanged);
            _character.Stat.AddListener(StatId.InventoryCapacity, OnInventoryCapacityStatChanged);
            _character.Stat.AddListener(StatId.ProjectileCapacity, OnProjectileCapacityStatChanged);
            _character.Stat.AddListener(StatId.Attack, OnAttackStatChanged);
            _character.Stat.AddListener(StatId.Defense, OnDefenseStatChanged);
            _character.Stat.AddListener(StatId.AttackRange, OnAttackRangeStatChanged);
            _character.Stat.AddListener(StatId.MoveSpeed, OnMoveSpeedStatChanged);
            _character.Stat.AddListener(StatId.AttackSpeed, OnAttackSpeedStatChanged);
            _character.Stat.AddListener(StatId.ReloadSpeed, OnReloadSpeedStatChanged);
            _character.Stat.AddListener(StatId.ProjectileSpeed, OnProjectileSpeedStatChanged);
            _character.Stat.AddListener(StatId.CriticalRate, OnCriticalRateStatChanged);
            _character.Stat.AddListener(StatId.CriticalDamage, OnCriticalDamageStatChanged);
            _character.Stat.AddListener(StatId.CriticalDamage, OnCriticalDamageStatChanged);
            _character.Stat.AddListener(StatId.Weight, OnWeightStatChanged);
            _character.Gauge.AddListener(StatId.Health, OnHealthGaugeChanged);
            _character.Gauge.AddListener(StatId.Hunger, OnHungerGaugeChanged);
            _character.Gauge.AddListener(StatId.Stamina, OnStaminaGaugeChanged);
            _character.Gauge.AddListener(StatId.Sanity, OnSanityGaugeChanged);
            _character.Gauge.AddListener(StatId.Weight, OnWeightGaugeChanged);
        }

        private void OnChangedEquipment(in NotifyCollectionChangedEventArgs<KeyValuePair<int, ItemEntity>> e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    SetItem(ItemSlotType.Equipment, e.NewItem.Key, e.NewItem.Value);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    ClearItem(ItemSlotType.Equipment, e.OldItem.Key);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    SetItem(ItemSlotType.Equipment, e.NewItem.Key, e.NewItem.Value);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                case NotifyCollectionChangedAction.Move:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnChangedInventory(in NotifyCollectionChangedEventArgs<KeyValuePair<int, ItemEntity>> e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    SetItem(ItemSlotType.Inventory, e.NewItem.Key, e.NewItem.Value);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    ClearItem(ItemSlotType.Inventory, e.OldItem.Key);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    SetItem(ItemSlotType.Inventory, e.NewItem.Key, e.NewItem.Value);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                case NotifyCollectionChangedAction.Move:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnChangedLoot(in NotifyCollectionChangedEventArgs<KeyValuePair<int, ItemEntity>> e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    SetItem(ItemSlotType.Loot, e.NewItem.Key, e.NewItem.Value);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    ClearItem(ItemSlotType.Loot, e.OldItem.Key);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    SetItem(ItemSlotType.Loot, e.NewItem.Key, e.NewItem.Value);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                case NotifyCollectionChangedAction.Move:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}