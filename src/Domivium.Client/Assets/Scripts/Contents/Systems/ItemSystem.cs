using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.DI;
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

        private readonly LocalDataService _localDataService;
        private readonly MasterDbService _masterDbService;
        private readonly Dictionary<(ItemType, int), int> _weightCache = new();
        private Items _items = new();

        public IReadOnlyObservableDictionary<int, ItemEntity> Equipment => _items.Equipment;
        public IReadOnlyObservableDictionary<int, ItemEntity> Inventory => _items.Inventory;
        public IReadOnlyObservableDictionary<int, ItemEntity> Storage => _items.Storage;
        public IReadOnlyObservableDictionary<int, ItemEntity> Loot => _items.Loot;

        public ReadOnlyReactiveProperty<int> InventoryCapacity => _items.InventoryCapacity;
        public ReadOnlyReactiveProperty<int> StorageCapacity => _items.StorageCapacity;
        public ReadOnlyReactiveProperty<int> LootCapacity => _items.LootCapacity;

        public ReadOnlyReactiveProperty<int> FilledInventoryCapacity => _items.FilledInventoryCapacity;
        public ReadOnlyReactiveProperty<int> FilledStorageCapacity => _items.FilledStorageCapacity;
        public ReadOnlyReactiveProperty<int> FilledLootCapacity => _items.FilledLootCapacity;

        public ReadOnlyReactiveProperty<int> LoadedProjectile => _items.LoadedProjectile;
        public ReadOnlyReactiveProperty<int> TotalProjectile => _items.TotalProjectile;

        public ReadOnlyReactiveProperty<int> TotalWeight => _items.TotalWeight;

        public ItemSystem(
            MasterDbService masterDbService,
            LocalDataService localDataService,
            ISubscriber<SceneMessage> sceneSubscriber)
        {
            _masterDbService = masterDbService;
            _localDataService = localDataService;
            _items.Equipment.CollectionChanged += OnChangedEquipment;
            _items.Inventory.CollectionChanged += OnChangedInventory;
            _items.Loot.CollectionChanged += OnChangedLoot;
            sceneSubscriber.Subscribe(OnSceneMessage).AddTo(ref DisposableBag);
        }

        protected override void OnDispose()
        {
            _localDataService.Save(_items);

            _items.Equipment.CollectionChanged -= OnChangedEquipment;
            _items.Inventory.CollectionChanged -= OnChangedInventory;
            _items.Loot.CollectionChanged -= OnChangedLoot;
            _items.Clear();

            base.OnDispose();
        }

        public bool TryGetItem(ItemSlotEntry slot, out ItemEntity item)
        {
            switch (slot.Type)
            {
                case ItemSlotType.Equipment:
                    return _items.Equipment.TryGetValue(slot.Index, out item);
                case ItemSlotType.Inventory:
                    return _items.Inventory.TryGetValue(slot.Index, out item);
                case ItemSlotType.Loot:
                    return _items.Loot.TryGetValue(slot.Index, out item);
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
                    for (var i = 0; i < _items.InventoryCapacity.CurrentValue; i++)
                    {
                        if (_items.Inventory.ContainsKey(i)) continue;

                        slotIndex = i;
                        return true;
                    }
                    return false;

                case ItemSlotType.Loot:
                    for (var i = 0; i < _items.LootCapacity.CurrentValue; i++)
                    {
                        if (_items.Loot.ContainsKey(i)) continue;

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

        public UniTask RunAsync()
        {
            if (_localDataService.Load(ref _items)) return UniTask.CompletedTask;

            SetLootCapacity(8);
            SetInventoryCapacity(16);
            Add(ItemSlotType.Inventory, new ItemEntity(Guid.NewGuid(), ItemType.Weapon, 1, 1));
            Add(ItemSlotType.Inventory, new ItemEntity(Guid.NewGuid(), ItemType.Weapon, 2, 1));
            Add(ItemSlotType.Inventory, new ItemEntity(Guid.NewGuid(), ItemType.Projectile, 1, 5));
            Add(ItemSlotType.Inventory, new ItemEntity(Guid.NewGuid(), ItemType.Projectile, 2, 77));
            Add(ItemSlotType.Inventory, new ItemEntity(Guid.NewGuid(), ItemType.Projectile, 1, 4));
            return UniTask.CompletedTask;
        }

        public void Equip(ItemSlotEntry fromSlot, ItemSlotEntry toSlot)
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

        public void Unequip(ItemSlotEntry fromSlot, ItemSlotEntry toSlot)
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
            _items.InventoryCapacity.Value = capacity;
            // todo: capacity보다 큰 인덱스 아이템은 자동 정리?
            // var removeList = new List<int>();
            // foreach (var (index, _) in _items.Inventory)
            // {
            //     if (index >= capacity) removeList.Add(index);
            // }
            // foreach (var index in removeList)
            // {
            //     _items.Inventory.Remove(index);
            // }
        }

        public void SetLootCapacity(int capacity)
        {
            _items.LootCapacity.Value = capacity;
        }

        public void Add(ItemSlotType slotType, ItemEntity item) // Except Equipment
        {
            if (!TryGetEmptySlotIndex(slotType, out var newSlotIndex)) return;

            SetInternal(new ItemSlotEntry(slotType, newSlotIndex), item);
        }

        public void Set(ItemSlotEntry slot, ItemEntity item)
        {
            if (!IsValid(slot)) return;

            SetInternal(slot, item);
        }

        public void SwapOrMerge(ItemSlotEntry fromSlot, ItemSlotEntry toSlot)
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

        public void SplitStack(ItemSlotEntry slot, int amount) // Only working in same slot type
        {
            if (!IsValid(slot)) return;

            if (!TryGetItem(slot, out var item)) return;

            if (!item.IsStackable) return;

            if (amount <= 0 || amount >= item.Count) return;

            if (!TryGetEmptySlotIndex(slot.Type, out var newSlotIndex)) return;

            var newItem = item.Split(amount);
            SetInternal(slot, item);
            SetInternal(new ItemSlotEntry(slot.Type, newSlotIndex), newItem);
        }

        public void Remove(ItemSlotEntry slot)
        {
            switch (slot.Type)
            {
                case ItemSlotType.Equipment:
                    _items.Equipment.Remove(slot.Index);
                    break;
                case ItemSlotType.Inventory:
                    _items.Inventory.Remove(slot.Index);
                    break;
                case ItemSlotType.Loot:
                    _items.Loot.Remove(slot.Index);
                    break;
                case ItemSlotType.None:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public bool UseProjectile()
        {
            if (!_items.Equipment.ContainsKey(ProjectileIndex)) return false;

            _items.Equipment[ProjectileIndex] = _items.Equipment[ProjectileIndex].Remove(1);
            if (_items.Equipment[ProjectileIndex].Count <= 0)
            {
                _items.Equipment.Remove(ProjectileIndex);
            }

            _items.LoadedProjectile.Value -= 1;
            return true;
        }

        public void Reload(int capacity)
        {
            if (capacity <= 0) return;

            var need = capacity - _items.LoadedProjectile.Value;
            var refill = _items.TotalProjectile.CurrentValue < need ? _items.TotalProjectile.CurrentValue : need;
            _items.TotalProjectile.Value -= refill;
            _items.LoadedProjectile.Value += refill;
        }

        private void SetInternal(ItemSlotEntry slot, ItemEntity item)
        {
            switch (slot.Type)
            {
                case ItemSlotType.Equipment:
                    _items.Equipment[slot.Index] = item;
                    break;
                case ItemSlotType.Inventory:
                    _items.Inventory[slot.Index] = item;
                    break;
                case ItemSlotType.Loot:
                    _items.Loot[slot.Index] = item;
                    break;
                case ItemSlotType.None:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Merge(ItemSlotEntry slot, int count)
        {
            switch (slot.Type)
            {
                case ItemSlotType.Equipment:
                    var equipmentItem = _items.Equipment[slot.Index];
                    equipmentItem.Add(count);
                    _items.Equipment[slot.Index] = equipmentItem;
                    break;
                case ItemSlotType.Inventory:
                    var inventoryItem = _items.Inventory[slot.Index];
                    inventoryItem.Add(count);
                    _items.Inventory[slot.Index] = inventoryItem;
                    break;
                case ItemSlotType.Loot:
                    var lootItem = _items.Loot[slot.Index];
                    lootItem.Add(count);
                    _items.Loot[slot.Index] = lootItem;
                    break;
                case ItemSlotType.None:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private bool IsValid(ItemSlotEntry slot)
        {
            switch (slot.Type)
            {
                case ItemSlotType.Equipment:
                    return slot.Index >= 0 && slot.Index < EquipmentCapacity;
                case ItemSlotType.Inventory:
                    return slot.Index >= 0 && slot.Index < _items.InventoryCapacity.CurrentValue;
                case ItemSlotType.Loot:
                    return slot.Index >= 0 && slot.Index < _items.LootCapacity.CurrentValue;
                case ItemSlotType.None:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private int GetTotalWeight(ItemEntity item)
        {
            var key = (item.Type, item.Id);
            if (_weightCache.TryGetValue(key, out var weight)) return weight * item.Count;

            var context = GetItemContext(item.Type, item.Id);
            _weightCache[key] = context.Weight;
            return context.Weight * item.Count;
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

        private void UpdateProjectile(ItemEntity item, bool unequip = false)
        {
            switch (item.Type)
            {
                case ItemType.Weapon:
                    _items.TotalProjectile.Value += _items.LoadedProjectile.Value;
                    _items.LoadedProjectile.Value = 0;
                    break;
                case ItemType.Projectile:
                    _items.TotalProjectile.Value = unequip ? 0 : item.Count;
                    _items.LoadedProjectile.Value = 0;
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
        }

        private void OnChangedInventory(in NotifyCollectionChangedEventArgs<KeyValuePair<int, ItemEntity>> e)
        {
            var newItem = e.NewItem.Value;
            var oldItem = e.OldItem.Value;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    _items.TotalWeight.Value += GetTotalWeight(newItem);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    _items.TotalWeight.Value -= GetTotalWeight(oldItem);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    _items.TotalWeight.Value -= GetTotalWeight(oldItem);
                    _items.TotalWeight.Value += GetTotalWeight(newItem);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                case NotifyCollectionChangedAction.Move:
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _items.FilledInventoryCapacity.Value = _items.Inventory.Count;
        }

        private void OnChangedEquipment(in NotifyCollectionChangedEventArgs<KeyValuePair<int, ItemEntity>> e)
        {
            var newItem = e.NewItem.Value;
            var oldItem = e.OldItem.Value;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    _items.TotalWeight.Value += GetTotalWeight(newItem);
                    UpdateProjectile(newItem);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    _items.TotalWeight.Value -= GetTotalWeight(oldItem);
                    UpdateProjectile(oldItem, true);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    _items.TotalWeight.Value -= GetTotalWeight(oldItem);
                    _items.TotalWeight.Value += GetTotalWeight(newItem);
                    if (oldItem.Guid != newItem.Guid)
                    {
                        UpdateProjectile(newItem);
                    }

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
            _items.FilledLootCapacity.Value = _items.Loot.Count;
        }

        private void OnSceneMessage(SceneMessage message)
        {
            switch (message.Type)
            {
                case SceneMessageType.Unload:
                case SceneMessageType.Load:
                    _items.Loot.Clear();
                    _items.TotalProjectile.Value += _items.LoadedProjectile.Value;
                    _items.LoadedProjectile.Value = 0;
                    _localDataService.Save(_items);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}