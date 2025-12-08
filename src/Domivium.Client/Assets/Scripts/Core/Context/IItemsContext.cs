using Domivium.Client.Data.DataTransferObject;
using Domivium.Client.Data.Item;
using ObservableCollections;
using R3;

namespace Domivium.Client.Core.Context
{
    public interface IItemsContext
    {
        public IReadOnlyObservableDictionary<int, ItemEntity> Equipment { get; }
        public IReadOnlyObservableDictionary<int, ItemEntity> Inventory { get; }
        public ReadOnlyReactiveProperty<int> FilledInventoryCapacity { get; }
        public ReadOnlyReactiveProperty<int> InventoryCapacity { get; }
        public ReadOnlyReactiveProperty<int> FilledWeightCapacity { get; }
        public ReadOnlyReactiveProperty<int> WeightCapacity { get; }
        public void Initialize(ItemsDto data);
        public bool TryToDto(out ItemsDto data);
        public bool TryGetInventoryEmptySlotIndex(out int slotIndex);
        public void MergeInventoryItem(int slotIndex, int count);
        public void SplitInventoryItem(int slotIndex, int count, int newSlotIndex);
        public bool UseEquipmentItem(int slotIndex, int count);
        public bool UseInventoryItem(int slotIndex, int count);
        public void SetEquipment(int slotIndex, ItemEntity item);
        public void SetInventory(int slotIndex, ItemEntity item);
        public void RemoveEquipment(int slotIndex);
        public void RemoveInventory(int slotIndex);
        public void ClearInventory();
        public void ClearEquipment();
        public void SetInventoryCapacity(int value);
        public void SetWeightCapacity(int value);
        public void AddInventoryCapacity(int value);
        public void AddWeightCapacity(int value);
    }
}