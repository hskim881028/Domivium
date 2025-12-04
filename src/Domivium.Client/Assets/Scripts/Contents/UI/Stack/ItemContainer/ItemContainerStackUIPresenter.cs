using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Services;
using Domivium.Client.Contents.UI.Contract;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Audio;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Container;
using Domivium.Client.Core.Systems;
using Domivium.Client.Core.UI.Contract;
using Domivium.Client.Core.UI.Navigation;
using Domivium.Client.Core.UI.Presenter;
using Domivium.Client.Core.Utility;
using Domivium.Client.Data.Item;
using Domivium.Client.Data.Loot;
using Domivium.Client.Data.Stat;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Domivium.Client.Contents.UI.Stack
{
    public class ItemContainerStackUIPresenter : StackUIPresenter<ItemContainerStackUIView, IItemContainerStackUIMessage>, IItemContainerStackUIMessage
    {
        private readonly ISpriteSystem _spriteSystem;
        private readonly IUserContainer _userContainer;
        private readonly MasterDbService _masterDbService;

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
            IUserContainer userContainer,
            MasterDbService masterDbService)
            : base(view, navigation, audioController)
        {
            actorManager.Character.Subscribe(OnChangeCharacter).AddTo(ref DisposableBag);
            view.SetUICamera(cameraSystem.UICamera);

            _spriteSystem = spriteSystem;
            _userContainer = userContainer;
            _masterDbService = masterDbService;

            _userContainer.Loot.Subscribe(OnChangedLoot).AddTo(ref DisposableBag);

            _userContainer.Equipment.CollectionChanged += OnChangedEquipment;
            _userContainer.Inventory.CollectionChanged += OnChangedInventory;

            _userContainer.FilledInventoryCapacity.Subscribe(View.SetFilledInventoryCapacity).AddTo(ref DisposableBag);
            _userContainer.InventoryCapacity.Subscribe(View.SetInventoryCapacity).AddTo(ref DisposableBag);

            _userContainer.FilledWeightCapacity.Subscribe(View.SetFilledWeightCapacity).AddTo(ref DisposableBag);
            _userContainer.WeightCapacity.Subscribe(View.SetWeightCapacity).AddTo(ref DisposableBag);

            _userContainer.Level.Subscribe(View.SetLevel).AddTo(ref DisposableBag);
            _userContainer.FilledExperience.Subscribe(View.SetFilledExperience).AddTo(ref DisposableBag);
            _userContainer.Experience.Subscribe(View.SetExperience).AddTo(ref DisposableBag);
            _userContainer = userContainer;

            foreach (var (index, item) in _userContainer.Equipment)
            {
                SetItem(ItemSlotType.Equipment, index, item);
            }

            foreach (var (index, item) in _userContainer.Inventory)
            {
                SetItem(ItemSlotType.Inventory, index, item);
            }
        }

        protected override void OnDispose()
        {
            _userContainer.Equipment.CollectionChanged -= OnChangedEquipment;
            _userContainer.Inventory.CollectionChanged -= OnChangedInventory;
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
        }

        public override void OnHideExit()
        {
            base.OnHideExit();
            _userContainer.Stop();
        }

        public void OnClick(ItemSlotEntry slot)
        {
            if (_userContainer.TryGetItem(slot, out var item) && !_selectedSlot.IsSame(slot))
            {
                var context = _masterDbService.GetItemTable(item.Type, item.Id);
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
            if (!_userContainer.TryGetItem(slot, out var item)) return;

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
                            _userContainer.Unequip(sourceSlot, targetSlot);
                            return;
                        case ItemSlotType.None:
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case ItemSlotType.Inventory:
                    switch (targetSlot.Type)
                    {
                        case ItemSlotType.Equipment:
                            _userContainer.Equip(sourceSlot, targetSlot);
                            return;
                        case ItemSlotType.Inventory:
                        case ItemSlotType.Loot:
                            _userContainer.SwapOrMergeItem(sourceSlot, targetSlot);
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
                            _userContainer.SwapOrMergeItem(sourceSlot, targetSlot);
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

            if (!_userContainer.TryGetItem(slot, out var item)) return;

            if (!Converter.TryGetEquipmentSlotIndex(item.Type, out var slotIndex)) return;

            var targetSlot = new ItemSlotEntry(ItemSlotType.Equipment, slotIndex);
            _userContainer.Equip(slot, targetSlot);
        }

        public void OnUnequip(ItemSlotEntry slot)
        {
            View.DeselectItem();

            if (!_userContainer.TryGetItem(slot, out _)) return;

            if (!_userContainer.TryGetEmptySlotIndex(ItemSlotType.Inventory, out var slotIndex)) return;

            var targetSlot = new ItemSlotEntry(ItemSlotType.Inventory, slotIndex);
            _userContainer.Unequip(slot, targetSlot);
        }

        public void OnShowSplitter(ItemSlotEntry slot)
        {
            if (!_userContainer.TryGetItem(slot, out var item)) return;

            View.ShowSplitter(slot, item.Count);
        }

        public void OnUse(ItemSlotEntry slot)
        {
            View.DeselectItem();
        }

        public void OnKeep(ItemSlotEntry slot)
        {
            View.DeselectItem();

            if (!_userContainer.TryGetEmptySlotIndex(_openType, out var slotIndex)) return;

            _userContainer.SwapOrMergeItem(slot, new ItemSlotEntry(_openType, slotIndex));
        }

        public void OnTakeOut(ItemSlotEntry slot)
        {
            View.DeselectItem();

            if (!_userContainer.TryGetEmptySlotIndex(ItemSlotType.Inventory, out var slotIndex)) return;

            _userContainer.SwapOrMergeItem(slot, new ItemSlotEntry(ItemSlotType.Inventory, slotIndex));
        }

        public void OnRemove(ItemSlotEntry slot)
        {
            View.DeselectItem();

            if (!_userContainer.TryGetItem(slot, out _)) return;

            _userContainer.RemoveItem(slot);
        }

        public void OnSplit(ItemSlotEntry slot, int count)
        {
            View.DeselectItem();
            _userContainer.SplitItem(slot, count);
        }

        private void SetItem(ItemSlotType slotType, int index, ItemEntity item)
        {
            var sprite = _spriteSystem.GetItem(item.Type, item.Id);
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

        private void OnChangedGauge(StatId statId)
        {
            var current = _character.Gauge.Current(statId);
            var limit = _character.Stat.Value(statId);
            View.SetGauge(statId, current, limit);
        }

        private void OnChangedStat(StatId statId)
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

        private void OnAttackStatChanged() => OnChangedStat(StatId.Attack);

        private void OnDefenseStatChanged() => OnChangedStat(StatId.Defense);

        private void OnPenetrationStatChanged() => OnChangedStat(StatId.Penetration);

        private void OnMoveSpeedStatChanged() => OnChangedStat(StatId.MoveSpeed);

        private void OnAttackRangeStatChanged() => OnChangedStat(StatId.AttackRange);

        private void OnAttackSpeedStatChanged() => OnChangedStat(StatId.AttackSpeed);

        private void OnProjectileSpeedStatChanged() => OnChangedStat(StatId.ProjectileSpeed);

        private void OnProjectileCapacityStatChanged() => OnChangedStat(StatId.ProjectileCapacity);

        private void OnReloadSpeedStatChanged() => OnChangedStat(StatId.ReloadSpeed);

        private void OnCriticalRateStatChanged() => OnChangedStat(StatId.CriticalRate);

        private void OnCriticalDamageStatChanged() => OnChangedStat(StatId.CriticalDamage);

        private void OnHealthGaugeChanged() => OnChangedGauge(StatId.Health);

        private void OnHungerGaugeChanged() => OnChangedGauge(StatId.Hunger);

        private void OnStaminaGaugeChanged() => OnChangedGauge(StatId.Stamina);

        private void OnSanityGaugeChanged() => OnChangedGauge(StatId.Sanity);

        private void OnChangeCharacter(IUnitPresenter character)
        {
            if (character == null) return;

            _character = character.BattleSystem;
            _character.Stat.AddListener(StatId.Attack, OnAttackStatChanged);
            _character.Stat.AddListener(StatId.Defense, OnDefenseStatChanged);
            _character.Stat.AddListener(StatId.Penetration, OnPenetrationStatChanged);
            _character.Stat.AddListener(StatId.MoveSpeed, OnMoveSpeedStatChanged);
            _character.Stat.AddListener(StatId.AttackRange, OnAttackRangeStatChanged);
            _character.Stat.AddListener(StatId.AttackSpeed, OnAttackSpeedStatChanged);
            _character.Stat.AddListener(StatId.ProjectileSpeed, OnProjectileSpeedStatChanged);
            _character.Stat.AddListener(StatId.ProjectileCapacity, OnProjectileCapacityStatChanged);
            _character.Stat.AddListener(StatId.ReloadSpeed, OnReloadSpeedStatChanged);
            _character.Stat.AddListener(StatId.CriticalRate, OnCriticalRateStatChanged);
            _character.Stat.AddListener(StatId.CriticalDamage, OnCriticalDamageStatChanged);

            _character.Gauge.AddListener(StatId.Health, OnHealthGaugeChanged);
            _character.Gauge.AddListener(StatId.Hunger, OnHungerGaugeChanged);
            _character.Gauge.AddListener(StatId.Stamina, OnStaminaGaugeChanged);
            _character.Gauge.AddListener(StatId.Sanity, OnSanityGaugeChanged);
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

        private void OnChangedLoot(LootEntity loot)
        {
            View.ClearLoot();
            View.SetFilledLootCapacity(loot.Items.Count);
            View.SetLootCapacity(loot.Capacity);
            foreach (var (slotIndex, item) in loot.Items)
            {
                SetItem(ItemSlotType.Loot, slotIndex, item);
            }
        }
    }
}