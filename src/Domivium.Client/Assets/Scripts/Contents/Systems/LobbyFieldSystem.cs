using Domivium.Client.Core.Systems;
using UnityEngine.Tilemaps;

namespace Domivium.Client.Contents.Systems
{
    public class LobbyFieldSystem : Disposable, ILobbyFieldSystem, ILobbyFieldSystemCommand
    {
        private Tilemap _grid;

        public void InitializeAsync(Tilemap grid)
        {
            _grid = grid;
        }
    }
}