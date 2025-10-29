using UnityEngine.Tilemaps;

namespace Domivium.Client.Contents.System.Command
{
    public interface IStageSystemCommand
    {
        public void InitializeAsync(Tilemap grid);
    }
}