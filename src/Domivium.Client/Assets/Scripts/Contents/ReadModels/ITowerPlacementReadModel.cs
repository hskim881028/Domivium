using ObservableCollections;
using R3;
using UnityEngine;

namespace Domivium.Client.Contents.ReadModels
{
    public interface ITowerPlacementReadModel
    {
        public ReadOnlyReactiveProperty<bool> Ready { get; }
        public IReadOnlyObservableList<Vector3Int> StagedTower { get; }
        public IReadOnlyObservableDictionary<Vector3Int, bool> PreviewTower { get; }
    }
}