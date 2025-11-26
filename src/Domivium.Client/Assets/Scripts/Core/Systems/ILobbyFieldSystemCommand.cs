using UnityEngine.Tilemaps;

namespace Domivium.Client.Core.Systems
{
    public interface ILobbyFieldSystemCommand
    {
        public void InitializeAsync(Tilemap grid);
    }
}