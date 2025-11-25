using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.UI.Contract;
using Domivium.Client.Core.Audio;
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
        private readonly ICharacterSystem _characterSystem;
        private readonly ICharacterSystemCommand _characterSystemCommand;
        private readonly IItemSystemCommand _itemSystemCommand;

        private ItemSlotData _selectedSlot = ItemSlotData.Default;
        private ItemSlotType _openType = ItemSlotType.None;

        public ItemContainerStackUIPresenter(
            ItemContainerStackUIView view,
            IUINavigation navigation,
            IAudioPlayer audioController,
            ICameraSystem cameraSystem,
            ISpriteSystem spriteSystem,
            ICharacterSystem characterSystem,
            IItemSystem itemSystem,
            ICharacterSystemCommand characterSystemCommand,
            IItemSystemCommand itemSystemCommand)
            : base(view, navigation, audioController)
        {
            view.SetUICamera(cameraSystem.UICamera);

            _spriteSystem = spriteSystem;
            _characterSystem = characterSystem;
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

        public override async UniTask<bool> InitializeAsync(CancellationToken token)
        {
            if (!await base.InitializeAsync(token)) return false;

            View.SetInventoryCapacity(_itemSystem.InventoryCapacity.CurrentValue);
            View.SetLootCapacity(_itemSystem.LootCapacity.CurrentValue);
            foreach (var (index, item) in _itemSystem.Inventory)
            {
                SetItem(ItemSlotType.Inventory, index, item);
            }

            _characterSystem.Character.Stat.AddListener(StatId.Health, OnHealthStatChanged);
            _characterSystem.Character.Stat.AddListener(StatId.Hunger, OnHungerStatChanged);
            _characterSystem.Character.Stat.AddListener(StatId.Stamina, OnStaminaStatChanged);
            _characterSystem.Character.Stat.AddListener(StatId.Sanity, OnSanityStatChanged);
            _characterSystem.Character.Stat.AddListener(StatId.Durability, OnDurabilityStatChanged);
            _characterSystem.Character.Stat.AddListener(StatId.InventoryCapacity, OnInventoryCapacityStatChanged);
            _characterSystem.Character.Stat.AddListener(StatId.ProjectileCapacity, OnProjectileCapacityStatChanged);
            _characterSystem.Character.Stat.AddListener(StatId.Attack, OnAttackStatChanged);
            _characterSystem.Character.Stat.AddListener(StatId.Defense, OnDefenseStatChanged);
            _characterSystem.Character.Stat.AddListener(StatId.AttackRange, OnAttackRangeStatChanged);
            _characterSystem.Character.Stat.AddListener(StatId.MoveSpeed, OnMoveSpeedStatChanged);
            _characterSystem.Character.Stat.AddListener(StatId.AttackSpeed, OnAttackSpeedStatChanged);
            _characterSystem.Character.Stat.AddListener(StatId.ReloadSpeed, OnReloadSpeedStatChanged);
            _characterSystem.Character.Stat.AddListener(StatId.ProjectileSpeed, OnProjectileSpeedStatChanged);
            _characterSystem.Character.Stat.AddListener(StatId.CriticalRate, OnCriticalRateStatChanged);
            _characterSystem.Character.Stat.AddListener(StatId.CriticalDamage, OnCriticalDamageStatChanged);
            _characterSystem.Character.Stat.AddListener(StatId.CriticalDamage, OnCriticalDamageStatChanged);
            _characterSystem.Character.Stat.AddListener(StatId.Weight, OnWeightStatChanged);

            _characterSystem.Character.Gauge.AddListener(StatId.Health, OnHealthGaugeChanged);
            _characterSystem.Character.Gauge.AddListener(StatId.Hunger, OnHungerGaugeChanged);
            _characterSystem.Character.Gauge.AddListener(StatId.Stamina, OnStaminaGaugeChanged);
            _characterSystem.Character.Gauge.AddListener(StatId.Sanity, OnSanityGaugeChanged);
            _characterSystem.Character.Gauge.AddListener(StatId.Weight, OnWeightGaugeChanged);
            return true;
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

        public void OnClick(ItemSlotData slot)
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
                _selectedSlot = ItemSlotData.Default;
            }
        }

        public void OnBeginDrag(ItemSlotData slot, Vector2 position)
        {
            if (!_itemSystem.TryGetItem(slot, out var item)) return;

            View.PickItem(slot, item.Type, position);
            _selectedSlot = slot;
        }

        public void OnDrag(Vector2 position)
        {
            View.MoveItem(position);
        }

        public void OnEndDrag(ItemSlotData sourceSlot, Vector2 position)
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

        public void OnEquip(ItemSlotData slot)
        {
            View.DeselectItem();

            if (!_itemSystem.TryGetItem(slot, out var item)) return;

            var targetSlot = new ItemSlotData(ItemSlotType.Equipment, Converter.GetSlotIndex(item.Type));
            _itemSystemCommand.Equip(slot, targetSlot);
        }

        public void OnUnequip(ItemSlotData slot)
        {
            View.DeselectItem();

            if (!_itemSystem.TryGetItem(slot, out _)) return;

            if (!_itemSystem.TryGetEmptySlotIndex(ItemSlotType.Inventory, out var slotIndex)) return;

            var targetSlot = new ItemSlotData(ItemSlotType.Inventory, slotIndex);
            _itemSystemCommand.Unequip(slot, targetSlot);
        }

        public void OnShowSplitter(ItemSlotData slot)
        {
            if (!_itemSystem.TryGetItem(slot, out var item)) return;

            View.ShowSplitter(slot, item.Count);
        }

        public void OnUse(ItemSlotData slot)
        {
            View.DeselectItem();
        }

        public void OnKeep(ItemSlotData slot)
        {
            View.DeselectItem();

            if (!_itemSystem.TryGetEmptySlotIndex(_openType, out var slotIndex)) return;

            _itemSystemCommand.SwapOrMerge(slot, new ItemSlotData(_openType, slotIndex));
        }

        public void OnTakeOut(ItemSlotData slot)
        {
            View.DeselectItem();

            if (!_itemSystem.TryGetEmptySlotIndex(ItemSlotType.Inventory, out var slotIndex)) return;

            _itemSystemCommand.SwapOrMerge(slot, new ItemSlotData(ItemSlotType.Inventory, slotIndex));
        }

        public void OnRemove(ItemSlotData slot)
        {
            View.DeselectItem();

            if (!_itemSystem.TryGetItem(slot, out _)) return;

            _itemSystemCommand.Remove(slot);
        }

        public void OnSplit(ItemSlotData slot, int count)
        {
            View.DeselectItem();
            _itemSystemCommand.SplitStack(slot, count);
        }

        private void SetItem(ItemSlotType slotType, int index, ItemData item)
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

        private void OnChangedEquipment(in NotifyCollectionChangedEventArgs<KeyValuePair<int, ItemData>> e)
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
                case NotifyCollectionChangedAction.Move:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnChangedInventory(in NotifyCollectionChangedEventArgs<KeyValuePair<int, ItemData>> e)
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
                case NotifyCollectionChangedAction.Move:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnChangedLoot(in NotifyCollectionChangedEventArgs<KeyValuePair<int, ItemData>> e)
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
                case NotifyCollectionChangedAction.Move:
                default:
                    throw new ArgumentOutOfRangeException();
            }
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
        private void OnWeightStatChanged() => SetWight();

        private void OnHealthGaugeChanged() => SetGauge(StatId.Health);

        private void OnHungerGaugeChanged() => SetGauge(StatId.Hunger);

        private void OnStaminaGaugeChanged() => SetGauge(StatId.Stamina);

        private void OnSanityGaugeChanged() => SetGauge(StatId.Sanity);

        private void OnWeightGaugeChanged() => SetWight();

        private void SetGauge(StatId statId)
        {
            var current = _characterSystem.Character.Gauge.Current(statId);
            var limit = _characterSystem.Character.Stat.Value(statId);
            View.SetGauge(statId, current, limit);
        }

        private void SetStat(StatId statId)
        {
            if (StatSet.GetDomain(statId) == StatDomain.Value)
            {
                View.SetStat(statId, _characterSystem.Character.Stat.Value(statId));
            }
            else
            {
                View.SetStat(statId, _characterSystem.Character.Stat.RateValue(statId));
            }
        }

        private void SetWight()
        {
            var current = _characterSystem.Character.Gauge.Current(StatId.Weight);
            var limit = _characterSystem.Character.Stat.Value(StatId.Weight);
            View.SetWeight(current, limit);
        }
    }
}