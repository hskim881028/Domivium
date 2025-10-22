using ObservableCollections;
using R3;

namespace Domivium.Client.Contents.ReadModels
{
    public interface IStageInventoryReadModel
    {
        public ReadOnlyReactiveProperty<int> RerollCost { get; }
        public ReadOnlyReactiveProperty<int> Soul { get; }
        public ReadOnlyReactiveProperty<int> Tower { get; }
        public ReadOnlyReactiveProperty<int> TowerLimit { get; }
        public IReadOnlyObservableDictionary<int, (int towerId, int cost)> TowerSlot { get; }
        public bool CanReroll();
        public bool CanPlacementTower(int slotIndex);
    }
}