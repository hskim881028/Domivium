using System;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

namespace Domivium.Client.Core.Provider
{
    public sealed class StageMapProvider
    {
        private readonly IReadOnlyList<Tilemap> _biome;

        public StageMapProvider(List<Tilemap> biome)
        {
            _biome = biome;
        }

        public Tilemap Get(int stageId)
        {
            // todo: stage를 Biome으로 묶기 
            var index = Math.Clamp(stageId - 1, 0, _biome.Count - 1);
            return _biome[index];
        }
    }
}