using UnityEngine.Tilemaps;

namespace Domivium.Client.Core.Systems
{
    public interface IStageSystemCommand
    {
        public void InitializeAsync(Tilemap grid);
    }
}