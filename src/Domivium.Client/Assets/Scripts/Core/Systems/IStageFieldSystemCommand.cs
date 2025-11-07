using UnityEngine.Tilemaps;

namespace Domivium.Client.Core.Systems
{
    public interface IStageFieldSystemCommand
    {
        public void InitializeAsync(Tilemap grid);
    }
}