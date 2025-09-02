using System;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

namespace Domivium.Client.Core.Provider
{
    public sealed class StageMapProvider
    {
        private readonly IReadOnlyList<Tilemap> _tilemaps;

        public StageMapProvider(List<Tilemap> tilemaps)
        {
            _tilemaps = tilemaps;
        }

        public Tilemap Get(int stageId)
        {
            var index = Math.Clamp(stageId - 1, 0, _tilemaps.Count - 1);
            return _tilemaps[index];
        }
    }
}