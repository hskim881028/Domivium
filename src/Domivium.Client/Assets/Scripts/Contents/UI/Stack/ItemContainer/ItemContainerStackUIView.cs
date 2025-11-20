using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.UI.Contract;
using Domivium.Client.Contents.UIComponents;
using Domivium.Client.Contents.UIComponents.ItemContainer;
using Domivium.Client.Core.UI.Contract;
using Domivium.Client.Core.UI.View;
using Domivium.Client.Core.Utility;
using Domivium.Client.Data.Context;
using Domivium.Client.Data.Item;
using UnityEngine;

namespace Domivium.Client.Contents.UI.Stack
{
    public class ItemContainerStackUIView : StackUIView<IItemContainerStackUIMessage>
    {
        [SerializeField] private Equipment _equipment;
        [SerializeField] private Inventory _inventory;
        [SerializeField] private Loot _loot;
        [SerializeField] private ItemVisual _stagedItem;
        [SerializeField] private ItemSelector _itemSelector;
        [SerializeField] private ItemSplitter _itemSplitter;
        [SerializeField] private ItemInformation _itemInformation;

        private Camera _uiCamera;
        private ItemSlotType _openType = ItemSlotType.None;

        public override async UniTask<bool> InitializeAsync(CancellationToken token)
        {
            if (!await base.InitializeAsync(token)) return false;

            Reset();

            _equipment.Initialize(_uiCamera, Message);
            _inventory.Initialize(_uiCamera, Message);
            _loot.Initialize(_uiCamera, Message);

            _itemSelector.Equip = Message.OnEquip;
            _itemSelector.Unequip = Message.OnUnequip;
            _itemSelector.Split = Message.OnShowSplitter;
            _itemSelector.Use = Message.OnUse;
            _itemSelector.Keep = Message.OnKeep;
            _itemSelector.TakeOut = Message.OnTakeOut;
            _itemSelector.Remove = Message.OnRemove;

            _itemSplitter.Split = Message.OnSplit;
            return true;
        }

        public override async UniTask ShowAsync(CancellationToken token, UIParam param, bool immediately = false)
        {
            await base.ShowAsync(token, param, immediately);

            Reset();
            switch (param)
            {
                case InventoryParams:
                    _openType = ItemSlotType.Inventory;
                    _loot.Hide();
                    break;
                case InventoryWithLootParams:
                    _openType = ItemSlotType.Loot;
                    _loot.Show();
                    break;
            }
        }

        public void SetInventoryCapacity(int capacity)
        {
            _inventory.SetInventoryCapacity(capacity);
        }

        public void SetLootCapacity(int capacity)
        {
            _loot.SetInventoryCapacity(capacity);
        }

        public void SetEquipmentSlot(int slotCount, int index, Sprite sprite, int count, bool isStackable)
        {
            _equipment.SetSlot(slotCount, index, sprite, count, isStackable);
        }

        public void SetInventorySlot(int slotCount, int index, Sprite sprite, int count, bool isStackable)
        {
            _inventory.SetSlot(slotCount, index, sprite, count, isStackable);
        }

        public void SetLootSlot(int slotCount, int index, Sprite sprite, int count, bool isStackable)
        {
            _loot.SetSlot(slotCount, index, sprite, count, isStackable);
        }

        public void ClearEquipmentSlot(int slotCount, int index)
        {
            _equipment.ClearSlot(slotCount, index);
        }

        public void ClearInventorySlot(int slotCount, int index)
        {
            _inventory.ClearSlot(slotCount, index);
        }

        public void ClearLootSlot(int slotCount, int index)
        {
            _loot.ClearSlot(slotCount, index);
        }

        public void SetUICamera(Camera uiCamera)
        {
            _uiCamera = uiCamera;
        }

        public void DeselectItem()
        {
            _itemInformation.Hide();
            _itemSelector.Hide();
            _itemSplitter.Hide();
            _equipment.ClearFocus();
        }

        public void SelectItem(ItemSlotData slot, ItemData item, ItemContext context)
        {
            // todo: 조건 추가 - 창고 on/off 상태
            var canKeep = _openType == ItemSlotType.Loot;
            var selectedSlot = GetSlot(slot);
            _itemSelector.Show(selectedSlot.RectTransform, slot, item.Type, canKeep);
            _itemInformation.Show(selectedSlot.Sprite, item, context);
            _itemSplitter.Hide();
            Focus(slot.Type, item.Type);
        }

        public void PickItem(ItemSlotData slot, ItemType itemType, Vector2 position)
        {
            _itemInformation.Hide();
            _itemSelector.Hide();
            _itemSplitter.Hide();

            var selectedSlot = GetSlot(slot);
            if (selectedSlot.IsStackable)
            {
                _stagedItem.Show(selectedSlot.Sprite, selectedSlot.Count);
            }
            else
            {
                _stagedItem.Show(selectedSlot.Sprite);
            }

            _stagedItem.SetPosition(position);

            switch (slot.Type)
            {
                case ItemSlotType.Equipment:
                    _loot.Lock();
                    break;
                case ItemSlotType.Inventory:
                    break;
                case ItemSlotType.Loot:
                    _equipment.Lock();
                    break;
                case ItemSlotType.None:
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Focus(slot.Type, itemType);
        }

        public void MoveItem(Vector2 position)
        {
            _stagedItem.SetPosition(position);
        }

        public void DropItem()
        {
            _stagedItem.Hide();
            _loot.Unlock();
            _equipment.Unlock();
            _equipment.ClearFocus();
        }

        public bool TryGetSlot(Vector2 position, out ItemSlotData slot)
        {
            slot = default;

            if (_equipment.TryGetSlot(position, out slot)) return true;

            if (_inventory.TryGetSlot(position, out slot)) return true;

            if (_loot.TryGetSlot(position, out slot)) return true;

            return false;
        }

        public void ShowSplitter(ItemSlotData slot, int itemCount)
        {
            _itemSplitter.Show(slot, itemCount);
        }

        private ItemSlot GetSlot(ItemSlotData slot)
        {
            switch (slot.Type)
            {
                case ItemSlotType.Equipment:
                    return _equipment.GetSlot(slot.Index);
                case ItemSlotType.Inventory:
                    return _inventory.GetSlot(slot.Index);
                case ItemSlotType.Loot:
                    return _loot.GetSlot(slot.Index);
                case ItemSlotType.None:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Focus(ItemSlotType slotType, ItemType itemType)
        {
            if (slotType != ItemSlotType.Inventory) return;

            var slotIndex = Converter.GetSlotIndex(itemType);
            _equipment.Focus(slotIndex);
        }

        private void Reset()
        {
            _itemInformation.Hide();
            _stagedItem.Hide();
            _itemSplitter.Hide();
            _itemSelector.Hide();

            _equipment.Reset();
            _inventory.Reset();
            _loot.Reset();
        }
    }
}