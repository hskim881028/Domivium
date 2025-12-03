using Cysharp.Threading.Tasks;
using Domivium.Client.Core.Battle;
using Domivium.Client.Data.Item;
using Domivium.Client.Data.Loot;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Domivium.Client.Core.Container
{
    public interface IUserContainer : ITicker
    {
        public ReadOnlyReactiveProperty<int> Level { get; }
        public ReadOnlyReactiveProperty<int> FilledExperience { get; }
        public ReadOnlyReactiveProperty<int> Experience { get; }

        public ReadOnlyReactiveProperty<int> LoadedProjectile { get; }
        public ReadOnlyReactiveProperty<int> RemainProjectile { get; }
        public ReadOnlyReactiveProperty<Vector2> OnTurn { get; }
        public ReadOnlyReactiveProperty<Vector2> OnLookAt { get; }
        public ReadOnlyReactiveProperty<BattleTag> OnBattleTag { get; }
        
        public IReadOnlyObservableDictionary<int, ItemEntity> Equipment { get; }
        public IReadOnlyObservableDictionary<int, ItemEntity> Inventory { get; }
        public ReadOnlyReactiveProperty<int> FilledInventoryCapacity { get; }
        public ReadOnlyReactiveProperty<int> InventoryCapacity { get; }
        public ReadOnlyReactiveProperty<int> FilledWeightCapacity { get; }
        public ReadOnlyReactiveProperty<int> WeightCapacity { get; }

        public ReadOnlyReactiveProperty<LootEntity> Loot { get; }
        public bool FoundLoot { get; }

        public UniTask InitializeUserAsync(int userId, int characterId);
        public UniTask InitializeItemAsync();
        public UniTask InitializeLootAsync(Transform character, int stageId);

        public UniTask SaveAsync();

        public bool TryGetItem(ItemSlotEntry slot, out ItemEntity item);
        public bool TryGetEmptySlotIndex(ItemSlotType slotType, out int slotIndex);
        public void Equip(ItemSlotEntry fromSlot, ItemSlotEntry toSlot);
        public void Unequip(ItemSlotEntry fromSlot, ItemSlotEntry toSlot);
        public void SwapOrMergeItem(ItemSlotEntry fromSlot, ItemSlotEntry toSlot);
        public void SplitItem(ItemSlotEntry slot, int count);
        public void RemoveItem(ItemSlotEntry slot);

        public bool Attack();
        public void Reload(int capacity);
        public void Stop();
        public bool SetDirection(Vector2 value);
        public bool LookAt(Vector2 value);
        public bool Avoid();
    }
}