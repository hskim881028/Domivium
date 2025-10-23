using Domivium.Client.Data.Info;
using R3;

namespace Domivium.Client.Contents.ReadModels
{
    public interface ITowerPlacementReadModel
    {
        public ReadOnlyReactiveProperty<bool> Ready { get; }
        public ReadOnlyReactiveProperty<StageCellInfo> PreviewTower { get; }
    }
}