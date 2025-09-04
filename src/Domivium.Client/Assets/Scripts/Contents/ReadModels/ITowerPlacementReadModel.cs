using ObservableCollections;
using UnityEngine;

namespace Domivium.Client.Contents.ReadModels
{
    public interface ITowerPlacementReadModel
    {
        public IReadOnlyObservableList<Vector3Int> StagedTower { get; }
        public IReadOnlyObservableDictionary<Vector3Int, bool> PreviewPreviewTower { get; }
    }
}