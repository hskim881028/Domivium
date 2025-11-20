using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Services;
using Domivium.Client.Contents.UI.Contract;
using Domivium.Client.Core.Audio;
using Domivium.Client.Core.Systems;
using Domivium.Client.Core.UI.Contract;
using Domivium.Client.Core.UI.Navigation;
using Domivium.Client.Core.UI.Presenter;
using Domivium.Client.Core.Utility;
using Domivium.Client.Data.Item;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Domivium.Client.Contents.UI.Stack
{
    public class ItemContainerStackUIPresenter : StackUIPresenter<ItemContainerStackUIView, IItemContainerStackUIMessage>, IItemContainerStackUIMessage
    {
        private readonly IItemSystem _itemSystem;
        private readonly MasterDbService _masterDbService;
        private readonly ISpriteSystem _spriteSystem;
        private readonly IItemSystemCommand _itemSystemCommand;

        private ItemSlotData _selectedSlot = ItemSlotData.Default;
        private ItemSlotType _openType = ItemSlotType.None;

        public ItemContainerStackUIPresenter(
            ItemContainerStackUIView view,
            IUINavigation navigation,
            IAudioPlayer audioController,
            ICameraSystem cameraSystem,
            ISpriteSystem spriteSystem,
            IItemSystemCommand itemSystemCommand,
            IItemSystem itemSystem,
            MasterDbService masterDbService)
            : base(view, navigation, audioController)
        {
            view.SetUICamera(cameraSystem.UICamera);
            _spriteSystem = spriteSystem;
            _itemSystemCommand = itemSystemCommand;
            _itemSystem = itemSystem;
            _masterDbService = masterDbService;
            _itemSystem.InventoryCapacity.Subscribe(View.SetInventoryCapacity).AddTo(ref DisposableBag);
            _itemSystem.LootCapacity.Subscribe(View.SetLootCapacity).AddTo(ref DisposableBag);
            _itemSystem.Equipment.CollectionChanged += OnChangedEquipment;
            _itemSystem.Inventory.CollectionChanged += OnChangedInventory;
            _itemSystem.Loot.CollectionChanged += OnChangedLoot;
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            _itemSystem.Equipment.CollectionChanged -= OnChangedEquipment;
            _itemSystem.Inventory.CollectionChanged -= OnChangedInventory;
            _itemSystem.Loot.CollectionChanged -= OnChangedLoot;
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
                    View.SetEquipmentSlot(_itemSystem.Equipment.Count, index, sprite, itemCount, isStackable);
                    break;
                case ItemSlotType.Inventory:
                    View.SetInventorySlot(_itemSystem.Inventory.Count, index, sprite, itemCount, isStackable);
                    break;
                case ItemSlotType.Loot:
                    View.SetLootSlot(_itemSystem.Loot.Count, index, sprite, itemCount, isStackable);
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
                    View.ClearEquipmentSlot(_itemSystem.Equipment.Count, index);
                    break;
                case ItemSlotType.Inventory:
                    View.ClearInventorySlot(_itemSystem.Inventory.Count, index);
                    break;
                case ItemSlotType.Loot:
                    View.ClearLootSlot(_itemSystem.Loot.Count, index);
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
    }
}