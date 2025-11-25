using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Domivium.Client.Contents.Services;
using Domivium.Client.Core.Message;
using Domivium.Client.Core.Systems;
using Domivium.Client.Core.Utility;
using Domivium.Client.Data.Context;
using Domivium.Client.Data.Item;
using MessagePipe;
using ObservableCollections;
using R3;

namespace Domivium.Client.Contents.Systems
{
    public sealed class ItemSystem : Disposable, IItemSystem, IItemSystemCommand, IItemUsageSystemCommand
    {
        private const int EquipmentCapacity = 7;
        private const int ProjectileIndex = 4;

        private readonly MasterDbService _masterDbService;
        private readonly ObservableDictionary<int, ItemData> _equipment = new();
        private readonly ObservableDictionary<int, ItemData> _inventory = new();
        private readonly ObservableDictionary<int, ItemData> _loot = new();

        private readonly ReactiveProperty<int> _filledInventoryCapacity = new();
        private readonly ReactiveProperty<int> _inventoryCapacity = new();
        private readonly ReactiveProperty<int> _filledLootCapacity = new();
        private readonly ReactiveProperty<int> _lootCapacity = new();

        private readonly ReactiveProperty<int> _loadedProjectile = new();
        private readonly ReactiveProperty<int> _totalProjectile = new();

        private readonly ReactiveProperty<int> _totalWeight = new();
        private readonly Dictionary<(ItemType, int), int> _weightCache = new();

        public IReadOnlyObservableDictionary<int, ItemData> Equipment => _equipment;
        public IReadOnlyObservableDictionary<int, ItemData> Inventory => _inventory;
        public IReadOnlyObservableDictionary<int, ItemData> Loot => _loot;
        public ReadOnlyReactiveProperty<int> FilledInventoryCapacity => _filledInventoryCapacity;
        public ReadOnlyReactiveProperty<int> InventoryCapacity => _inventoryCapacity;
        public ReadOnlyReactiveProperty<int> FilledLootCapacity => _filledLootCapacity;
        public ReadOnlyReactiveProperty<int> LootCapacity => _lootCapacity;
        public ReadOnlyReactiveProperty<int> LoadedProjectile => _loadedProjectile;
        public ReadOnlyReactiveProperty<int> TotalProjectile => _totalProjectile;
        public ReadOnlyReactiveProperty<int> TotalWeight => _totalWeight;

        public ItemSystem(
            MasterDbService masterDbService,
            ISubscriber<SceneMessage> sceneSubscriber)
        {
            _masterDbService = masterDbService;
            sceneSubscriber.Subscribe(OnSceneMessage).AddTo(ref DisposableBag);

            _equipment.CollectionChanged += OnChangedEquipment;
            _inventory.CollectionChanged += OnChangedInventory;
            _loot.CollectionChanged += OnChangedLoot;
        }

        protected override void OnDispose()
        {
            _equipment.CollectionChanged -= OnChangedEquipment;
            _inventory.CollectionChanged -= OnChangedInventory;
            _loot.CollectionChanged -= OnChangedLoot;
            base.OnDispose();
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

            if (!TryGetItem(fromSlot, out var fromItem)) return;

            var toItemType = Converter.GetItemType(toSlot.Index);
            if (fromItem.Type != toItemType) return;

            if (TryGetItem(toSlot, out var toItem)) // swap
            {
                SetInternal(toSlot, fromItem);
                SetInternal(fromSlot, toItem);
            }
            else // move
            {
                SetInternal(toSlot, fromItem);
                Remove(fromSlot);
            }
        }

        public void Unequip(ItemSlotData fromSlot, ItemSlotData toSlot)
        {
            if (!IsValid(fromSlot) || !IsValid(toSlot)) return;

            if (fromSlot.IsSame(toSlot)) return;

            if (!TryGetItem(fromSlot, out var fromItem)) return;

            if (TryGetItem(toSlot, out var toItem)) // swap
            {
                if (fromItem.Type != toItem.Type) return;

                SetInternal(toSlot, fromItem);
                SetInternal(fromSlot, toItem);
            }
            else // move
            {
                SetInternal(toSlot, fromItem);
                Remove(fromSlot);
            }
        }

        public void SetInventoryCapacity(int capacity)
        {
            _inventoryCapacity.Value = capacity;
            // todo: capacity보다 큰 인덱스 아이템은 자동 정리?
            // var removeList = new List<int>();
            // foreach (var (index, _) in _inventory)
            // {
            //     if (index >= capacity) removeList.Add(index);
            // }
            // foreach (var index in removeList)
            // {
            //     _inventory.Remove(index);
            // }
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

            if (!TryGetItem(fromSlot, out var fromItem)) return;

            if (TryGetItem(toSlot, out var toItem))
            {
                if (toItem.CanMerge(fromItem)) // merge
                {
                    Merge(toSlot, fromItem.Count);
                    Remove(fromSlot);
                }
                else // swap
                {
                    SetInternal(toSlot, fromItem);
                    SetInternal(fromSlot, toItem);
                }
            }
            else // move
            {
                SetInternal(toSlot, fromItem);
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
                    Unequip(slot);
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

        public bool UseProjectile()
        {
            if (!_equipment.ContainsKey(ProjectileIndex)) return false;

            _equipment[ProjectileIndex] = _equipment[ProjectileIndex].Remove(1);
            if (_equipment[ProjectileIndex].Count <= 0)
            {
                _equipment.Remove(ProjectileIndex);
            }

            _loadedProjectile.Value -= 1;
            return true;
        }

        public void Reload(int capacity)
        {
            if (capacity <= 0) return;

            var need = capacity - _loadedProjectile.Value;
            var refill = _totalProjectile.CurrentValue < need ? _totalProjectile.CurrentValue : need;
            _totalProjectile.Value -= refill;
            _loadedProjectile.Value += refill;
        }

        private void SetInternal(ItemSlotData slot, ItemData item)
        {
            switch (slot.Type)
            {
                case ItemSlotType.Equipment:
                    Equip(slot, item);
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

        private void Equip(ItemSlotData slot, ItemData item)
        {
            switch (item.Type)
            {
                case ItemType.Weapon:
                    _totalProjectile.Value += _loadedProjectile.Value;
                    _loadedProjectile.Value = 0;
                    break;
                case ItemType.Projectile:
                    _totalProjectile.Value = item.Count;
                    _loadedProjectile.Value = 0;
                    break;
                case ItemType.Helmet:
                case ItemType.Necklace:
                case ItemType.Backpack:
                case ItemType.Armor:
                case ItemType.Ring:
                case ItemType.Food:
                case ItemType.Potion:
                    break;
                case ItemType.None:
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _equipment[slot.Index] = item;
        }

        private void Unequip(ItemSlotData slot)
        {
            var item = _equipment[slot.Index];
            switch (item.Type)
            {
                case ItemType.Weapon:
                    _totalProjectile.Value += _loadedProjectile.CurrentValue;
                    _loadedProjectile.Value = 0;
                    break;
                case ItemType.Projectile:
                    _totalProjectile.Value = 0;
                    _loadedProjectile.Value = 0;
                    break;
                case ItemType.Helmet:
                case ItemType.Necklace:
                case ItemType.Backpack:
                case ItemType.Armor:
                case ItemType.Ring:
                case ItemType.Food:
                case ItemType.Potion:
                    break;
                case ItemType.None:
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _equipment.Remove(slot.Index);
        }

        private int GetWeight(ItemData item)
        {
            var key = (item.Type, item.Id);
            if (_weightCache.TryGetValue(key, out var weight)) return weight;

            var context = GetItemContext(item.Type, item.Id);
            _weightCache[key] = context.Weight;
            return context.Weight;
        }

        private void AddWeight(ItemData item)
        {
            _totalWeight.Value += item.Count * GetWeight(item);
        }

        private void SubtractWeight(ItemData item)
        {
            _totalWeight.Value -= item.Count * GetWeight(item);
        }

        private void OnChangedInventory(in NotifyCollectionChangedEventArgs<KeyValuePair<int, ItemData>> e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    AddWeight(e.NewItem.Value);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    SubtractWeight(e.OldItem.Value);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    SubtractWeight(e.OldItem.Value);
                    AddWeight(e.NewItem.Value);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                case NotifyCollectionChangedAction.Move:
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _filledInventoryCapacity.Value = _inventory.Count;
        }

        private void OnChangedEquipment(in NotifyCollectionChangedEventArgs<KeyValuePair<int, ItemData>> e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    AddWeight(e.NewItem.Value);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    SubtractWeight(e.OldItem.Value);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    SubtractWeight(e.OldItem.Value);
                    AddWeight(e.NewItem.Value);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                case NotifyCollectionChangedAction.Move:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnChangedLoot(in NotifyCollectionChangedEventArgs<KeyValuePair<int, ItemData>> e)
        {
            _filledLootCapacity.Value = _loot.Count;
        }

        private void OnSceneMessage(SceneMessage message)
        {
            switch (message.Type)
            {
                case SceneMessageType.Unload:
                case SceneMessageType.Load:
                    _equipment.Clear();
                    _inventory.Clear();
                    _loot.Clear();
                    _filledInventoryCapacity.Value = 0;
                    _inventoryCapacity.Value = 0;
                    _filledLootCapacity.Value = 0;
                    _lootCapacity.Value = 0;
                    _loadedProjectile.Value = 0;
                    _totalProjectile.Value = 0;
                    _totalWeight.Value = 0;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}