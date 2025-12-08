using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Domivium.Client.Contents.Services;
using Domivium.Client.Core.Context;
using Domivium.Client.Data.DataTransferObject;
using Domivium.Client.Data.Item;
using ObservableCollections;
using R3;

namespace Domivium.Client.Contents.Context
{
    public sealed class ItemsContext : Disposable, IItemsContext
    {
        private readonly MasterDbService _masterDbService;
        private readonly Items _items = new();
        private readonly ReactiveProperty<int> _filledInventoryCapacity = new();
        private readonly ReactiveProperty<int> _inventoryCapacity = new();
        private readonly ReactiveProperty<int> _filledWeightCapacity = new();
        private readonly ReactiveProperty<int> _weightCapacity = new();

        private readonly Dictionary<(ItemType, int), int> _weightCache = new();

        public IReadOnlyObservableDictionary<int, ItemEntity> Equipment => _items.Equipment;
        public IReadOnlyObservableDictionary<int, ItemEntity> Inventory => _items.Inventory;
        public ReadOnlyReactiveProperty<int> FilledInventoryCapacity => _filledInventoryCapacity;
        public ReadOnlyReactiveProperty<int> InventoryCapacity => _inventoryCapacity;
        public ReadOnlyReactiveProperty<int> FilledWeightCapacity => _filledWeightCapacity;
        public ReadOnlyReactiveProperty<int> WeightCapacity => _weightCapacity;

        public ItemsContext(MasterDbService masterDbService)
        {
            _masterDbService = masterDbService;
            _items.Equipment.CollectionChanged += OnChangedEquipment;
            _items.Inventory.CollectionChanged += OnChangedInventory;
        }

        protected override void OnDispose()
        {
            _items.Equipment.CollectionChanged -= OnChangedEquipment;
            _items.Inventory.CollectionChanged -= OnChangedInventory;
            base.OnDispose();
        }

        public void Initialize(ItemsDto data)
        {
            _items.SetData(data);
        }

        public void ClearInventory()
        {
            _items.ClearInventory();
        }

        public void ClearEquipment()
        {
            _items.ClearEquipment();
        }

        public bool TryToDto(out ItemsDto data) => _items.ToDto(out data);

        public bool TryGetInventoryEmptySlotIndex(out int slotIndex)
        {
            slotIndex = -1;
            for (var i = 0; i < _inventoryCapacity.CurrentValue; i++)
            {
                if (_items.Inventory.ContainsKey(i)) continue;

                slotIndex = i;
                return true;
            }

            return false;
        }

        public void MergeInventoryItem(int slotIndex, int count)
        {
            _items.MergeInventoryItem(slotIndex, count);
        }

        public void SplitInventoryItem(int slotIndex, int count, int newSlotIndex)
        {
            _items.SplitInventoryItem(slotIndex, count, newSlotIndex);
        }

        public bool UseEquipmentItem(int slotIndex, int count) => _items.UseEquipmentItem(slotIndex, count);

        public bool UseInventoryItem(int slotIndex, int count) => _items.UseInventoryItem(slotIndex, count);

        public void SetEquipment(int slotIndex, ItemEntity item)
        {
            _items.SetEquipment(slotIndex, item);
        }

        public void SetInventory(int slotIndex, ItemEntity item)
        {
            _items.SetInventory(slotIndex, item);
        }

        public void RemoveEquipment(int slotIndex)
        {
            _items.RemoveEquipment(slotIndex);
        }

        public void RemoveInventory(int slotIndex)
        {
            _items.RemoveInventory(slotIndex);
        }

        public void SetInventoryCapacity(int value)
        {
            _inventoryCapacity.Value = value;
        }

        public void SetWeightCapacity(int value)
        {
            _weightCapacity.Value = value;
        }

        public void AddInventoryCapacity(int value)
        {
            _inventoryCapacity.Value += value;
        }

        public void AddWeightCapacity(int value)
        {
            _weightCapacity.Value += value;
        }

        private int GetTotalWeight(ItemEntity item)
        {
            var key = (item.Type, item.Id);
            if (_weightCache.TryGetValue(key, out var weight)) return weight * item.Count;

            var context = _masterDbService.GetItemTable(item.Type, item.Id);
            _weightCache[key] = context.Weight;
            return context.Weight * item.Count;
        }

        private void OnChangedInventory(in NotifyCollectionChangedEventArgs<KeyValuePair<int, ItemEntity>> e)
        {
            var newItem = e.NewItem.Value;
            var oldItem = e.OldItem.Value;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    _filledWeightCapacity.Value += GetTotalWeight(newItem);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    _filledWeightCapacity.Value -= GetTotalWeight(oldItem);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    _filledWeightCapacity.Value -= GetTotalWeight(oldItem);
                    _filledWeightCapacity.Value += GetTotalWeight(newItem);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                case NotifyCollectionChangedAction.Move:
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _filledInventoryCapacity.Value = _items.Inventory.Count;
        }

        private void OnChangedEquipment(in NotifyCollectionChangedEventArgs<KeyValuePair<int, ItemEntity>> e)
        {
            var newItem = e.NewItem.Value;
            var oldItem = e.OldItem.Value;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    _filledWeightCapacity.Value += GetTotalWeight(newItem);

                    break;
                case NotifyCollectionChangedAction.Remove:
                    _filledWeightCapacity.Value -= GetTotalWeight(oldItem);

                    break;
                case NotifyCollectionChangedAction.Replace:
                    _filledWeightCapacity.Value -= GetTotalWeight(oldItem);
                    _filledWeightCapacity.Value += GetTotalWeight(newItem);

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