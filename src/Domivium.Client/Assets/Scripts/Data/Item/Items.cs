using System;
using Domivium.Client.Data.DataTransferObject;
using ObservableCollections;

namespace Domivium.Client.Data.Item
{
    public sealed class Items
    {
        private readonly ObservableDictionary<int, ItemEntity> _equipment = new();
        private readonly ObservableDictionary<int, ItemEntity> _inventory = new();
        private readonly ObservableDictionary<int, ItemEntity> _storage = new();

        private int _userId;
        private int _characterId;
        private bool _confirmed;

        public IReadOnlyObservableDictionary<int, ItemEntity> Equipment => _equipment;
        public IReadOnlyObservableDictionary<int, ItemEntity> Inventory => _inventory;
        public IReadOnlyObservableDictionary<int, ItemEntity> Storage => _storage;

        public void SetData(ItemsDto data)
        {
            _confirmed = true;
            _userId = data.UserId;
            _characterId = data.CharacterId;

            _equipment.Clear();
            foreach (var item in data.Equipment)
            {
                _equipment[item.SlotIndex] = new ItemEntity(item.Guid, item.ItemType, item.ItemId, item.ItemCount, item.IsStackable);
            }

            _inventory.Clear();
            foreach (var item in data.Inventory)
            {
                _inventory[item.SlotIndex] = new ItemEntity(item.Guid, item.ItemType, item.ItemId, item.ItemCount, item.IsStackable);
            }

            _storage.Clear();
            foreach (var item in data.Storage)
            {
                _storage[item.SlotIndex] = new ItemEntity(item.Guid, item.ItemType, item.ItemId, item.ItemCount, item.IsStackable);
            }
        }

        public void MergeInventoryItem(int slotIndex, int count)
        {
            var item = _inventory[slotIndex].AddCount(count);
            _inventory.Remove(slotIndex);
            _inventory.Add(slotIndex, item);
        }

        public void SplitInventoryItem(int slotIndex, int count, int newSlotIndex)
        {
            var (oldItem, newItem) = _inventory[slotIndex].Split(count);
            _inventory.Remove(slotIndex);
            _inventory.Add(slotIndex, oldItem);
            _inventory.Add(newSlotIndex, newItem);
        }

        public bool UseEquipmentItem(int slotIndex, int count)
        {
            if (_equipment[slotIndex].Count < count) return false;

            var item = _equipment[slotIndex].RemoveCount(count);
            _equipment.Remove(slotIndex);
            if (item.Count > 0)
            {
                _equipment.Add(slotIndex, item);
            }
            return true;
        }

        public bool UseInventoryItem(int slotIndex, int count)
        {
            if (_inventory[slotIndex].Count < count) return false;

            var item = _inventory[slotIndex].RemoveCount(count);
            _inventory.Remove(slotIndex);
            if (item.Count > 0)
            {
                _inventory.Add(slotIndex, item);
            }
            return true;
        }

        public void SetEquipment(int slotIndex, ItemEntity item)
        {
            _equipment[slotIndex] = item;
        }

        public void SetInventory(int slotIndex, ItemEntity item)
        {
            _inventory[slotIndex] = item;
        }

        public void RemoveEquipment(int slotIndex)
        {
            _equipment.Remove(slotIndex);
        }

        public void RemoveInventory(int slotIndex)
        {
            _inventory.Remove(slotIndex);
        }

        public bool ToDto(out ItemsDto data)
        {
            data = new ItemsDto();
            if (!_confirmed) return false;

            data.Modified = DateTime.UtcNow;
            data.UserId = _userId;
            data.CharacterId = _characterId;

            foreach (var (slotIndex, entity) in _equipment)
            {
                data.Equipment.Add(entity.ToDto(slotIndex));
            }

            foreach (var (slotIndex, entity) in _inventory)
            {
                data.Inventory.Add(entity.ToDto(slotIndex));
            }

            foreach (var (slotIndex, entity) in _storage)
            {
                data.Storage.Add(entity.ToDto(slotIndex));
            }

            return true;
        }
    }
}