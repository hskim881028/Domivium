using System;
using Domivium.Client.Contents.Services;
using Domivium.Client.Core.Systems;
using Domivium.Client.Core.Utility;
using Domivium.Client.Data.Context;
using Domivium.Client.Data.Item;
using ObservableCollections;
using R3;

namespace Domivium.Client.Contents.Systems
{
    public sealed class ItemSystem : IItemSystem, IItemSystemCommand
    {
        private const int EquipmentCapacity = 7;

        private readonly MasterDbService _masterDbService;
        private readonly ObservableDictionary<int, ItemData> _equipment = new();
        private readonly ObservableDictionary<int, ItemData> _inventory = new();
        private readonly ObservableDictionary<int, ItemData> _loot = new();
        private readonly ReactiveProperty<int> _inventoryCapacity = new();
        private readonly ReactiveProperty<int> _lootCapacity = new();

        public IReadOnlyObservableDictionary<int, ItemData> Equipment => _equipment;
        public IReadOnlyObservableDictionary<int, ItemData> Inventory => _inventory;
        public IReadOnlyObservableDictionary<int, ItemData> Loot => _loot;
        public ReadOnlyReactiveProperty<int> InventoryCapacity => _inventoryCapacity;
        public ReadOnlyReactiveProperty<int> LootCapacity => _lootCapacity;

        public ItemSystem(MasterDbService masterDbService)
        {
            _masterDbService = masterDbService;
        }

        public ItemContext GetItemContext(ItemType itemType, int itemId)
        {
            switch (itemType)
            {
                case ItemType.Weapon:
                    return new ItemContext(_masterDbService.DB.WeaponRowTable.FindById(itemId));
                case ItemType.Projectile:
                    return new ItemContext(_masterDbService.DB.ProjectileRowTable.FindById(itemId));
                case ItemType.Helmet:
                case ItemType.Necklace:
                case ItemType.Backpack:
                case ItemType.Armor:
                case ItemType.Ring:
                case ItemType.Food:
                case ItemType.Potion:
                case ItemType.None:
                default:
                    throw new ArgumentOutOfRangeException(nameof(itemType), itemType, null);
            }
        }

        public bool TryGetItem(ItemSlotData slot, out ItemData item)
        {
            switch (slot.Type)
            {
                case ItemSlotType.Equipment:
                    return _equipment.TryGetValue(slot.Index, out item);
                case ItemSlotType.Inventory:
                    return _inventory.TryGetValue(slot.Index, out item);
                case ItemSlotType.Loot:
                    return _loot.TryGetValue(slot.Index, out item);
                case ItemSlotType.None:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public bool TryGetEmptySlotIndex(ItemSlotType slotType, out int slotIndex)
        {
            slotIndex = -1;
            switch (slotType)
            {
                case ItemSlotType.Inventory:
                    for (var i = 0; i < _inventoryCapacity.CurrentValue; i++)
                    {
                        if (_inventory.ContainsKey(i)) continue;

                        slotIndex = i;
                        return true;
                    }
                    return false;

                case ItemSlotType.Loot:
                    for (var i = 0; i < _lootCapacity.CurrentValue; i++)
                    {
                        if (_loot.ContainsKey(i)) continue;

                        slotIndex = i;
                        return true;
                    }
                    return false;
                case ItemSlotType.Equipment:
                case ItemSlotType.None:
                default:
                    throw new ArgumentOutOfRangeException(nameof(slotType), slotType, null);
            }
        }

        public void Equip(ItemSlotData fromSlot, ItemSlotData toSlot)
        {
            if (!IsValid(fromSlot) || !IsValid(toSlot)) return;

            if (fromSlot.IsSame(toSlot)) return;

            if (!TryGetItem(fromSlot, out var formItem)) return;

            var toItemType = Converter.GetItemType(toSlot.Index);
            if (formItem.Type != toItemType) return;

            if (TryGetItem(toSlot, out var toItem)) // swap
            {
                SetInternal(toSlot, formItem);
                SetInternal(fromSlot, toItem);
            }
            else // move
            {
                SetInternal(toSlot, formItem);
                Remove(fromSlot);
            }
        }

        public void Unequip(ItemSlotData fromSlot, ItemSlotData toSlot)
        {
            if (!IsValid(fromSlot) || !IsValid(toSlot)) return;

            if (fromSlot.IsSame(toSlot)) return;

            if (!TryGetItem(fromSlot, out var formItem)) return;

            if (TryGetItem(toSlot, out var toItem)) // swap
            {
                if (formItem.Type != toItem.Type) return;

                SetInternal(toSlot, formItem);
                SetInternal(fromSlot, toItem);
            }
            else // move
            {
                SetInternal(toSlot, formItem);
                Remove(fromSlot);
            }
        }

        public void SetInventoryCapacity(int capacity)
        {
            _inventoryCapacity.Value = capacity;
        }

        public void SetLootCapacity(int capacity)
        {
            _lootCapacity.Value = capacity;
        }

        public void Add(ItemSlotType slotType, ItemData item) // Except Equipment
        {
            if (!TryGetEmptySlotIndex(slotType, out var newSlotIndex)) return;

            SetInternal(new ItemSlotData(slotType, newSlotIndex), item);
        }

        public void Set(ItemSlotData slot, ItemData item)
        {
            if (!IsValid(slot)) return;

            SetInternal(slot, item);
        }

        public void SwapOrMerge(ItemSlotData fromSlot, ItemSlotData toSlot)
        {
            if (!IsValid(fromSlot) || !IsValid(toSlot)) return;

            if (fromSlot.IsSame(toSlot)) return;

            if (!TryGetItem(fromSlot, out var formItem)) return;

            if (TryGetItem(toSlot, out var toItem))
            {
                if (toItem.CanMerge(formItem)) // merge
                {
                    Merge(toSlot, formItem.Count);
                    Remove(fromSlot);
                }
                else // swap
                {
                    SetInternal(toSlot, formItem);
                    SetInternal(fromSlot, toItem);
                }
            }
            else // move
            {
                SetInternal(toSlot, formItem);
                Remove(fromSlot);
            }
        }

        public void SplitStack(ItemSlotData slot, int amount) // Only working in same slot type
        {
            if (!IsValid(slot)) return;

            if (!TryGetItem(slot, out var item)) return;

            if (!item.IsStackable) return;

            if (amount <= 0 || amount >= item.Count) return;

            if (!TryGetEmptySlotIndex(slot.Type, out var newSlotIndex)) return;

            var newItem = item.Split(amount);
            SetInternal(slot, item);
            SetInternal(new ItemSlotData(slot.Type, newSlotIndex), newItem);
        }

        public void Remove(ItemSlotData slot)
        {
            switch (slot.Type)
            {
                case ItemSlotType.Equipment:
                    _equipment.Remove(slot.Index);
                    break;
                case ItemSlotType.Inventory:
                    _inventory.Remove(slot.Index);
                    break;
                case ItemSlotType.Loot:
                    _loot.Remove(slot.Index);
                    break;
                case ItemSlotType.None:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void SetInternal(ItemSlotData slot, ItemData item)
        {
            switch (slot.Type)
            {
                case ItemSlotType.Equipment:
                    _equipment[slot.Index] = item;
                    break;
                case ItemSlotType.Inventory:
                    _inventory[slot.Index] = item;
                    break;
                case ItemSlotType.Loot:
                    _loot[slot.Index] = item;
                    break;
                case ItemSlotType.None:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Merge(ItemSlotData slot, int count)
        {
            switch (slot.Type)
            {
                case ItemSlotType.Equipment:
                    var equipmentItem = _equipment[slot.Index];
                    equipmentItem.Add(count);
                    _equipment[slot.Index] = equipmentItem;
                    break;
                case ItemSlotType.Inventory:
                    var inventoryItem = _inventory[slot.Index];
                    inventoryItem.Add(count);
                    _inventory[slot.Index] = inventoryItem;
                    break;
                case ItemSlotType.Loot:
                    var lootItem = _loot[slot.Index];
                    lootItem.Add(count);
                    _loot[slot.Index] = lootItem;
                    break;
                case ItemSlotType.None:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private bool IsValid(ItemSlotData slot)
        {
            switch (slot.Type)
            {
                case ItemSlotType.Equipment:
                    return slot.Index >= 0 && slot.Index < EquipmentCapacity;
                case ItemSlotType.Inventory:
                    return slot.Index >= 0 && slot.Index < _inventoryCapacity.CurrentValue;
                case ItemSlotType.Loot:
                    return slot.Index >= 0 && slot.Index < _lootCapacity.CurrentValue;
                case ItemSlotType.None:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}