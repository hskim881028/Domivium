using System.Collections.Generic;
using Domivium.Client.Data.Loot;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Domivium.Client.Core.Systems
{
    public interface IStageFieldSystemCommand
    {
        public void InitializeForLobbyAsync(Tilemap grid);
        public void InitializeForStageAsync(Tilemap grid, IReadOnlyDictionary<LootType, IReadOnlyList<(ushort lootId, Vector2 position)>> loots);
    }
}