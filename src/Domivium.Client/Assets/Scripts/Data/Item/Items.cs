using ObservableCollections;
using R3;

namespace Domivium.Client.Data.Item
{
    public class Items
    {
        public ObservableDictionary<int, ItemEntity> Equipment { get; } = new();
        public ObservableDictionary<int, ItemEntity> Inventory { get; } = new();
        public ObservableDictionary<int, ItemEntity> Storage { get; } = new();
        public ObservableDictionary<int, ItemEntity> Loot { get; } = new();

        public ReactiveProperty<int> InventoryCapacity { get; } = new();
        public ReactiveProperty<int> StorageCapacity { get; } = new();
        public ReactiveProperty<int> LootCapacity { get; } = new();

        public ReactiveProperty<int> FilledInventoryCapacity { get; } = new();
        public ReactiveProperty<int> FilledStorageCapacity { get; } = new();

        public ReactiveProperty<int> FilledLootCapacity { get; } = new();

        public ReactiveProperty<int> LoadedProjectile { get; } = new();
        public ReactiveProperty<int> TotalProjectile { get; } = new();

        public ReactiveProperty<int> TotalWeight { get; } = new();

        public void Clear()
        {
            Equipment.Clear();
            Inventory.Clear();
            Storage.Clear();
            Loot.Clear();

            InventoryCapacity.Value = 0;
            StorageCapacity.Value = 0;
            LootCapacity.Value = 0;

            FilledInventoryCapacity.Value = 0;
            FilledStorageCapacity.Value = 0;
            FilledLootCapacity.Value = 0;

            LoadedProjectile.Value = 0;
            TotalProjectile.Value = 0;

            TotalWeight.Value = 0;
        }
    }
}