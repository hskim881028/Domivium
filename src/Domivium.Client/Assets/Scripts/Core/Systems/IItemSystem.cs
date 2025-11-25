using Domivium.Client.Data.Context;
using Domivium.Client.Data.Item;
using ObservableCollections;
using R3;

namespace Domivium.Client.Core.Systems
{
    public interface IItemSystem
    {
        public IReadOnlyObservableDictionary<int, ItemData> Equipment { get; }
        public IReadOnlyObservableDictionary<int, ItemData> Inventory { get; }
        public IReadOnlyObservableDictionary<int, ItemData> Loot { get; }
        public ReadOnlyReactiveProperty<int> FilledInventoryCapacity { get; }
        public ReadOnlyReactiveProperty<int> InventoryCapacity { get; }
        public ReadOnlyReactiveProperty<int> FilledLootCapacity { get; }
        public ReadOnlyReactiveProperty<int> LootCapacity { get; }
        public ReadOnlyReactiveProperty<int> LoadedProjectile { get; }
        public ReadOnlyReactiveProperty<int> TotalProjectile { get; }
        public ReadOnlyReactiveProperty<int> TotalWeight { get; }

        public ItemContext GetItemContext(ItemType itemType, int itemId);
        public bool TryGetItem(ItemSlotData slot, out ItemData item);
        public bool TryGetEmptySlotIndex(ItemSlotType slotType, out int slotIndex);
    }
}