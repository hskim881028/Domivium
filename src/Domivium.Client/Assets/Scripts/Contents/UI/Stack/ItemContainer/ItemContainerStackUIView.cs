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
using Domivium.Client.Data.Stat;
using UnityEngine;

namespace Domivium.Client.Contents.UI.Stack
{
    public class ItemContainerStackUIView : StackUIView<IItemContainerStackUIMessage>
    {
        [SerializeField] private Wallet _wallet;
        [SerializeField] private Status _status;
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
                    _status.Show();
                    _loot.Hide();
                    break;
                case InventoryWithLootParams:
                    _openType = ItemSlotType.Loot;
                    _status.Hide();
                    _loot.Show();
                    break;
            }
        }

        public void SetFilledWeightCapacity(int capacity)
        {
            _wallet.SetFilledWeightCapacity(capacity);
        }

        public void SetWeightCapacity(int capacity)
        {
            _wallet.SetWeightCapacity(capacity);
        }

        public void SetFilledInventoryCapacity(int capacity)
        {
            _inventory.SetFilledCapacity(capacity);
        }

        public void SetInventoryCapacity(int capacity)
        {
            _inventory.SetCapacity(capacity);
        }

        public void SetFilledLootCapacity(int capacity)
        {
            _loot.SetFilledCapacity(capacity);
        }

        public void SetLootCapacity(int capacity)
        {
            _loot.SetCapacity(capacity);
        }

        public void SetEquipmentSlot(int index, Sprite sprite, int count, bool isStackable)
        {
            _equipment.SetSlot(index, sprite, count, isStackable);
        }

        public void SetInventorySlot(int index, Sprite sprite, int count, bool isStackable)
        {
            _inventory.SetSlot(index, sprite, count, isStackable);
        }

        public void SetLootSlot(int index, Sprite sprite, int count, bool isStackable)
        {
            _loot.SetSlot(index, sprite, count, isStackable);
        }

        public void ClearEquipmentSlot(int index)
        {
            _equipment.ClearSlot(index);
        }

        public void ClearInventorySlot(int index)
        {
            _inventory.ClearSlot(index);
        }

        public void ClearLootSlot(int index)
        {
            _loot.ClearSlot(index);
        }

        public void ClearLoot()
        {
            _loot.Clear();
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

        public void SelectItem(ItemSlotEntry slot, ItemEntity item, ItemTable table)
        {
            // todo: 조건 추가 - 창고 on/off 상태
            var canKeep = _openType == ItemSlotType.Loot;
            var selectedSlot = GetSlot(slot);
            _itemSelector.Show(selectedSlot.RectTransform, slot, item.Type, canKeep);
            _itemInformation.Show(selectedSlot.Sprite, item, table);
            _itemSplitter.Hide();
            Focus(slot.Type, item.Type);
        }

        public void PickItem(ItemSlotEntry slot, ItemType itemType, Vector2 position)
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

        public bool TryGetSlot(Vector2 position, out ItemSlotEntry slot)
        {
            slot = default;

            if (_equipment.TryGetSlot(position, out slot)) return true;

            if (_inventory.TryGetSlot(position, out slot)) return true;

            if (_loot.TryGetSlot(position, out slot)) return true;

            return false;
        }

        public void ShowSplitter(ItemSlotEntry slot, int itemCount)
        {
            _itemSplitter.Show(slot, itemCount);
        }

        public void SetLevel(int value)
        {
            _status.SetLevel(value);
        }

        public void SetFilledExperience(int value)
        {
            _status.SetFilledExperience(value);
        }

        public void SetExperience(int value)
        {
            _status.SetExperience(value);
        }

        public void SetStat(StatId statId, float value)
        {
            _status.SetStat(statId, value);
        }

        public void SetGauge(StatId statId, int current, int limit)
        {
            _status.SetGauge(statId, current, limit);
        }

        public void SetMoney(int value)
        {
            _wallet.SetMoney(value);
        }

        public void SetGem(int value)
        {
            _wallet.SetGem(value);
        }

        private ItemSlot GetSlot(ItemSlotEntry slot)
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

            if (!Converter.TryGetEquipmentSlotIndex(itemType, out var slotIndex)) return;

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