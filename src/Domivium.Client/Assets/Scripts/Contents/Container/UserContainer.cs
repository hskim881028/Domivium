using System;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Services;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Container;
using Domivium.Client.Core.Context;
using Domivium.Client.Core.Message;
using Domivium.Client.Core.Utility;
using Domivium.Client.Data.Item;
using Domivium.Client.Data.Loot;
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

        private readonly IUserContext _user;
        private readonly IBattleContext _battle;
        private readonly IItemsContext _items;
        private readonly ILootsContext _loots;

        public ReadOnlyReactiveProperty<int> Level => _user.Level;
        public ReadOnlyReactiveProperty<int> FilledExperience => _user.FilledExperience;
        public ReadOnlyReactiveProperty<int> Experience => _user.Experience;
        
        public ReadOnlyReactiveProperty<int> LoadedProjectile => _battle.LoadedProjectile;
        public ReadOnlyReactiveProperty<int> RemainProjectile => _battle.RemainProjectile;
        public ReadOnlyReactiveProperty<Vector2> OnTurn => _battle.OnTurn;
        public ReadOnlyReactiveProperty<Vector2> OnLookAt => _battle.OnLookAt;
        public ReadOnlyReactiveProperty<BattleTag> OnBattleTag => _battle.OnBattleTag;
        
        public IReadOnlyObservableDictionary<int, ItemEntity> Equipment => _items.Equipment;
        public IReadOnlyObservableDictionary<int, ItemEntity> Inventory => _items.Inventory;
        public ReadOnlyReactiveProperty<int> FilledInventoryCapacity => _items.FilledInventoryCapacity;
        public ReadOnlyReactiveProperty<int> InventoryCapacity => _items.InventoryCapacity;
        public ReadOnlyReactiveProperty<int> FilledWeightCapacity => _items.FilledWeightCapacity;
        public ReadOnlyReactiveProperty<int> WeightCapacity => _items.WeightCapacity;

        public ReadOnlyReactiveProperty<LootEntity> Loot => _loots.Loot;
        public bool FoundLoot => _loots.FoundLoot;

        public UserContainer(
            MasterDbService masterDbService,
            LocalDataService localDataService,
            IUserContext user,
            IBattleContext battle,
            IItemsContext items,
            ILootsContext loots,
            ISubscriber<SceneMessage> sceneSubscriber)
        {
            _masterDbService = masterDbService;
            _localDataService = localDataService;
            _user = user;
            _battle = battle;
            _items = items;
            _loots = loots;
            sceneSubscriber.Subscribe(OnSceneMessage).AddTo(ref DisposableBag);
        }

        public async UniTask InitializeUserAsync(int userId, int characterId)
        {
            var data = await _localDataService.LoadUserAsync(userId, characterId);
            _user.Initialize(data);

            var characterRow = _masterDbService.DB.CharacterRowTable.FindById(characterId);
            _items.SetInventoryCapacity(characterRow.InventoryCapacity);
            _items.SetWeightCapacity(characterRow.WeightCapacity);
        }

        public async UniTask InitializeItemAsync()
        {
            var data = await _localDataService.LoadItemAsync(_user.Id, _user.CharacterId);
            _items.Initialize(data);
            foreach (var (_, item) in _items.Equipment)
            {
                OnChangedEquipState(item);
            }
        }

        public async UniTask InitializeLootAsync(Transform character, int stageId)
        {
            var data = await _localDataService.LoadLootAsync(_user.Id, _user.CharacterId, stageId);
            _loots.Initialize(character, data);
        }

        public async UniTask SaveAsync()
        {
            if (_user.TryToDto(out var userData))
            {
                await _localDataService.SaveUserAsync(userData);
            }

            if (_items.TryToDto(out var itemsData))
            {
                await _localDataService.SaveItemAsync(itemsData);
            }

            if (_loots.TryToDto(out var lootsData))
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
                    return _loots.TryGetItem(slot.Index, out item);
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
                    return _items.TryGetInventoryEmptySlotIndex(out slotIndex);
                case ItemSlotType.Loot:
                    return _loots.TryGetEmptySlotIndex(out slotIndex);
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
                RemoveItem(fromSlot);
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
                RemoveItem(fromSlot);
                OnChangedEquipState(fromItem, true);
            }
        }

        public void SwapOrMergeItem(ItemSlotEntry fromSlot, ItemSlotEntry toSlot)
        {
            if (!IsValid(fromSlot) || !IsValid(toSlot)) return;

            if (fromSlot.IsSame(toSlot)) return;

            if (!TryGetItem(fromSlot, out var fromItem)) return;

            if (TryGetItem(toSlot, out var toItem))
            {
                if (toItem.CanMerge(fromItem)) // merge
                {
                    Merge(toSlot, fromItem.Count);
                    RemoveItem(fromSlot);
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
                RemoveItem(fromSlot);
            }
        }

        public void SplitItem(ItemSlotEntry slot, int count)
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
                    RemoveItem(slot);
                    SetInternal(slot, changed);
                    SetInternal(new ItemSlotEntry(slot.Type, newSlotIndex), newItem);
                    break;
                case ItemSlotType.None:
                case ItemSlotType.Storage:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void RemoveItem(ItemSlotEntry slot)
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
                    _loots.Remove(slot.Index);
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

            _battle.Attack();
            return true;
        }

        public void Reload(int capacity)
        {
            _battle.Reload(capacity);
        }

        public void Stop()
        {
            _battle.Stop();
        }

        public bool SetDirection(Vector2 value) => _battle.SetDirection(value);

        public bool LookAt(Vector2 value) => _battle.LookAt(value);

        public bool Avoid() => _battle.Avoid();

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
                    _loots.Set(slot.Index, item);
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
                    _loots.Merge(slot.Index, count);
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
                    return slot.Index >= 0 && slot.Index < _items.InventoryCapacity.CurrentValue;
                case ItemSlotType.Loot:
                    return _loots.IsValid(slot.Index);
                case ItemSlotType.None:
                case ItemSlotType.Storage:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Tick(float deltaTime)
        {
            _loots.Tick(deltaTime);
        }

        private void OnChangedEquipState(ItemEntity item, bool unequip = false)
        {
            switch (item.Type)
            {
                case ItemType.Weapon:
                    _battle.RestoreProjectile();
                    break;
                case ItemType.Projectile:
                    _battle.SetProjectile(unequip ? 0 : item.Count);
                    break;
                case ItemType.Bag:
                    var row = _masterDbService.DB.BagRowTable.FindById(item.Id);
                    if (unequip)
                    {
                        _items.AddInventoryCapacity(-row.InventoryCapacity);
                        _items.AddWeightCapacity(-row.WeightCapacity);
                    }
                    else
                    {
                        _items.AddInventoryCapacity(row.InventoryCapacity);
                        _items.AddWeightCapacity(row.WeightCapacity);
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

        private void OnSceneMessage(SceneMessage message)
        {
            switch (message.Type)
            {
                case SceneMessageType.Unload:
                case SceneMessageType.Load:
                    _battle.RestoreProjectile();
                    Stop();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}