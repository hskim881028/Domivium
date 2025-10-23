using Domivium.Client.Core.Battle;
using Domivium.Client.Data.Info;
using Domivium.Client.Data.StageField;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Domivium.Client.Contents.ReadModels
{
    public interface IStageInventoryReadModel
    {
        public ReadOnlyReactiveProperty<int> RerollCost { get; }
        public ReadOnlyReactiveProperty<int> Soul { get; }
        public ReadOnlyReactiveProperty<int> TowerLimit { get; }
        public IReadOnlyObservableDictionary<int, TowerSlotInfo> TowerSlot { get; }
        public IReadOnlyObservableDictionary<Vector3Int, IBattleSystem> Towers { get; }
        public IReadOnlyObservableList<Vector3Int> Barrier { get; }
        public bool CanReroll();
        public bool CanSelectTower(int slotIndex);
        public bool CanMove(Vector3Int cell);
        public StageCellTag GetCellTag(Vector3Int cell, int slotIndex);
    }
}