using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Battle;
using Domivium.Client.Contents.Services;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Container;
using Domivium.Client.Core.Message;
using Domivium.Client.Core.Utility;
using Domivium.Client.Data.Item;
using Domivium.Client.Data.Loot;
using Domivium.Client.Data.User;
using MessagePipe;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Domivium.Client.Contents.Container
{
    public sealed class UserContainer : Disposable, IUserContainer
    {
        private readonly MasterDbService _masterDbService;
        private readonly LocalDataService _localDataService;

        private readonly User _user = new();
        private readonly Items _items = new();
        private readonly Loots _loots = new();

        private readonly ReactiveProperty<LootEntity> _loot = new();
        
        private readonly ReactiveProperty<int> _filledInventoryCapacity = new();
        private readonly ReactiveProperty<int> _inventoryCapacity = new();
        private readonly ReactiveProperty<int> _filledWeightCapacity = new();
        private readonly ReactiveProperty<int> _weightCapacity = new();
        
        private readonly ReactiveProperty<int> _loadedProjectile = new();
        private readonly ReactiveProperty<int> _remainProjectile = new();
        
        private readonly ReactiveProperty<int> _level = new();
        private readonly ReactiveProperty<int> _filledExperience = new();
        private readonly ReactiveProperty<int> _experience = new();

        private readonly ReactiveProperty<Vector2> _direction = new();
        private readonly ReactiveProperty<Vector2> _lookAt = new();

        private readonly Dictionary<(ItemType, int), int> _weightCache = new();

        public IReadOnlyObservableDictionary<int, ItemEntity> Equipment => _items.Equipment;
        public IReadOnlyObservableDictionary<int, ItemEntity> Inventory => _items.Inventory;
        public ReadOnlyReactiveProperty<LootEntity> Loot => _loot;
        public ReadOnlyReactiveProperty<int> FilledInventoryCapacity => _filledInventoryCapacity;
        public ReadOnlyReactiveProperty<int> InventoryCapacity => _inventoryCapacity;
        public ReadOnlyReactiveProperty<int> FilledWeightCapacity => _filledWeightCapacity;
        public ReadOnlyReactiveProperty<int> WeightCapacity => _weightCapacity;
        
        public ReadOnlyReactiveProperty<int> LoadedProjectile => _loadedProjectile;
        public ReadOnlyReactiveProperty<int> RemainProjectile => _remainProjectile;
        
        public ReadOnlyReactiveProperty<int> Level => _level;
        public ReadOnlyReactiveProperty<int> FilledExperience => _filledExperience;
        public ReadOnlyReactiveProperty<int> Experience => _experience;
        public ReactiveCommand<BattleTag> OnBattleTag { get; } = new();
        public ReadOnlyReactiveProperty<Vector2> OnTurn => _direction;
        public ReadOnlyReactiveProperty<Vector2> OnLookAt => _lookAt;

        public bool FoundLoot => _loot.CurrentValue != null;

        public UserContainer(
            MasterDbService masterDbService,
            LocalDataService localDataService,
            ISubscriber<SceneMessage> sceneSubscriber)
        {
            _items.Inventory.CollectionChanged += OnChangedInventory;
            _items.Equipment.CollectionChanged += OnChangedEquipment;
            _masterDbService = masterDbService;
            _localDataService = localDataService;
            sceneSubscriber.Subscribe(OnSceneMessage).AddTo(ref DisposableBag);
        }

        protected override void OnDispose()
        {
            _items.Inventory.CollectionChanged -= OnChangedInventory;
            _items.Equipment.CollectionChanged -= OnChangedEquipment;
            base.OnDispose();
        }

        public async UniTask InitializeUserAsync(int userId, int characterId)
        {
            var data = await _localDataService.LoadUserAsync(userId, characterId);
            _user.SetData(data);
            var characterRow = _masterDbService.DB.CharacterRowTable.FindById(_user.CharacterId);
            _inventoryCapacity.Value = characterRow.InventoryCapacity;
            _weightCapacity.Value = characterRow.WeightCapacity;
            _level.Value = _user.Level;
            _filledExperience.Value = _user.Experience;
            _experience.Value = _user.Level * 100;
        }

        public async UniTask InitializeItemAsync()
        {
            var data = await _localDataService.LoadItemAsync(_user.Id, _user.CharacterId);
            _items.SetData(data);
            foreach (var (_, item) in _items.Equipment)
            {
                OnChangedEquipState(item);
            }
        }

        public async UniTask InitializeLootAsync(Transform character, int stageId)
        {
            var data = await _localDataService.LoadLootAsync(_user.Id, _user.CharacterId, stageId);
            _loots.SetData(character, data);
        }

        public async UniTask SaveAsync()
        {
            if (_user.ToDto(out var userData))
            {
                await _localDataService.SaveUserAsync(userData);
            }
            if (_items.ToDto(out var itemsData))
            {
                await _localDataService.SaveItemAsync(itemsData);
            }
            if (_loots.ToDto(out var lootsData))
            {
                await _localDataService.SaveLootAsync(lootsData);
            }
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
                    item = null;
                    return _loot.CurrentValue != null && _loot.CurrentValue.Items.TryGetValue(slot.Index, out item);
                case ItemSlotType.None:
                case ItemSlotType.Storage:
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
                        if (_items.Inventory.ContainsKey(i)) continue;

                        slotIndex = i;
                        return true;
                    }
                    return false;

                case ItemSlotType.Loot:
                    if (_loot.CurrentValue == null) return false;

                    for (var i = 0; i < _loot.CurrentValue.Capacity; i++)
                    {
                        if (_loot.CurrentValue.Items.ContainsKey(i)) continue;

                        slotIndex = i;
                        return true;
                    }
                    return false;
                case ItemSlotType.Equipment:
                case ItemSlotType.None:
                case ItemSlotType.Storage:
                default:
                    throw new ArgumentOutOfRangeException(nameof(slotType), slotType, null);
            }
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

            OnChangedEquipState(fromItem);
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
                OnChangedEquipState(toItem);
            }
            else // move
            {
                SetInternal(toSlot, fromItem);
                Remove(fromSlot);
                OnChangedEquipState(fromItem, true);
            }
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

        public void SplitStack(ItemSlotEntry slot, int count)
        {
            if (!IsValid(slot)) return;

            if (!TryGetItem(slot, out var item)) return;

            if (!item.IsStackable) return;

            if (count <= 0 || count >= item.Count) return;

            if (!TryGetEmptySlotIndex(slot.Type, out var newSlotIndex)) return;

            switch (slot.Type)
            {
                case ItemSlotType.Equipment:
                    break;
                case ItemSlotType.Inventory:
                    _items.SplitInventoryItem(slot.Index, count, newSlotIndex);
                    break;
                case ItemSlotType.Loot:
                    var changed = new ItemEntity(Guid.NewGuid(), item.Type, item.Id, item.Count - count, item.IsStackable);
                    var newItem = new ItemEntity(Guid.NewGuid(), item.Type, item.Id, count, item.IsStackable);
                    Remove(slot);
                    SetInternal(slot, changed);
                    SetInternal(new ItemSlotEntry(slot.Type, newSlotIndex), newItem);
                    break;
                case ItemSlotType.None:
                case ItemSlotType.Storage:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Remove(ItemSlotEntry slot)
        {
            switch (slot.Type)
            {
                case ItemSlotType.Equipment:
                    _items.RemoveEquipment(slot.Index);
                    break;
                case ItemSlotType.Inventory:
                    _items.RemoveInventory(slot.Index);
                    break;
                case ItemSlotType.Loot:
                    if (_loot.CurrentValue == null) return;

                    _loot.CurrentValue.Items.Remove(slot.Index);
                    _loot.ForceNotify();
                    break;
                case ItemSlotType.None:
                case ItemSlotType.Storage:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public bool Attack()
        {
            if (!_items.UseEquipmentItem(Constant.ProjectileIndex, 1)) return false;

            _loadedProjectile.Value -= 1;
            return true;
        }

        public void Reload(int capacity)
        {
            if (capacity <= 0) return;

            var need = capacity - _loadedProjectile.Value;
            var refill = _remainProjectile.CurrentValue < need ? _remainProjectile.CurrentValue : need;
            _remainProjectile.Value -= refill;
            _loadedProjectile.Value += refill;
        }

        public void Stop()
        {
            _direction.Value = Vector2.zero;
            _lookAt.Value = Vector2.zero;
            OnBattleTag.Execute(BattleTags.Idle);
        }

        public bool SetDirection(Vector2 value)
        {
            if (value.sqrMagnitude > 1f)
            {
                value.Normalize();
            }

            _direction.Value = value;
            return true;
        }

        public bool LookAt(Vector2 value)
        {
            _lookAt.Value = value;
            switch (_lookAt.Value.sqrMagnitude)
            {
                case > 0 when value.sqrMagnitude <= Constant.CanAttackRange:
                    OnBattleTag.Execute(BattleTags.Aiming);
                    break;
                case > Constant.CanAttackRange:
                    OnBattleTag.Execute(BattleTags.Firing);
                    break;
                default:
                    OnBattleTag.Execute(BattleTags.Idle);
                    break;
            }

            return true;
        }

        public bool Avoid()
        {
            OnBattleTag.Execute(BattleTags.Avoid);
            return true;
        }

        private void SetInternal(ItemSlotEntry slot, ItemEntity item)
        {
            switch (slot.Type)
            {
                case ItemSlotType.Equipment:
                    _items.SetEquipment(slot.Index, item);
                    break;
                case ItemSlotType.Inventory:
                    _items.SetInventory(slot.Index, item);
                    break;
                case ItemSlotType.Loot:
                    if (_loot.CurrentValue == null) return;

                    _loot.CurrentValue.Items[slot.Index] = item;
                    _loot.ForceNotify();
                    break;
                case ItemSlotType.None:
                case ItemSlotType.Storage:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Merge(ItemSlotEntry slot, int count)
        {
            switch (slot.Type)
            {
                case ItemSlotType.Inventory:
                    _items.MergeInventoryItem(slot.Index, count);
                    break;
                case ItemSlotType.Loot:
                    if (_loot.CurrentValue == null) return;

                    _loot.CurrentValue.Items[slot.Index] = _loot.CurrentValue.Items[slot.Index].AddCount(count);
                    _loot.ForceNotify();
                    break;
                case ItemSlotType.None:
                case ItemSlotType.Equipment:
                case ItemSlotType.Storage:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private bool IsValid(ItemSlotEntry slot)
        {
            switch (slot.Type)
            {
                case ItemSlotType.Equipment:
                    return slot.Index is >= 0 and < Constant.EquipmentCapacity;
                case ItemSlotType.Inventory:
                    return slot.Index >= 0 && slot.Index < _inventoryCapacity.CurrentValue;
                case ItemSlotType.Loot:
                    var capacity = _loot.CurrentValue?.Capacity ?? 0;
                    return slot.Index >= 0 && slot.Index < capacity;
                case ItemSlotType.None:
                case ItemSlotType.Storage:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Tick(float deltaTime)
        {
            _loot.Value = _loots.FindNearestLoot();
        }

        private int GetTotalWeight(ItemEntity item)
        {
            var key = (item.Type, item.Id);
            if (_weightCache.TryGetValue(key, out var weight)) return weight * item.Count;

            var context = _masterDbService.GetItemContext(item.Type, item.Id);
            _weightCache[key] = context.Weight;
            return context.Weight * item.Count;
        }

        private void OnChangedEquipState(ItemEntity item, bool unequip = false)
        {
            switch (item.Type)
            {
                case ItemType.Weapon:
                    _remainProjectile.Value += _loadedProjectile.Value;
                    _loadedProjectile.Value = 0;
                    break;
                case ItemType.Projectile:
                    _remainProjectile.Value = unequip ? 0 : item.Count;
                    _loadedProjectile.Value = 0;
                    break;
                case ItemType.Bag:
                    var row = _masterDbService.DB.BagRowTable.FindById(item.Id);
                    if (unequip)
                    {
                        _inventoryCapacity.Value -= row.InventoryCapacity;
                        _weightCapacity.Value -= row.WeightCapacity;
                    }
                    else
                    {
                        _inventoryCapacity.Value += row.InventoryCapacity;
                        _weightCapacity.Value += row.WeightCapacity;
                    }

                    break;
                case ItemType.Ring:
                case ItemType.Necklace:
                case ItemType.Head:
                case ItemType.Body:
                case ItemType.Feet:
                    break;
                case ItemType.Potion:
                case ItemType.Food:
                case ItemType.Cash:
                case ItemType.Material:
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

        private void OnSceneMessage(SceneMessage message)
        {
            switch (message.Type)
            {
                case SceneMessageType.Unload:
                case SceneMessageType.Load:
                    _remainProjectile.Value += _loadedProjectile.Value;
                    _loadedProjectile.Value = 0;
                    Stop();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}