using System;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

namespace Domivium.Client.Core.Provider
{
    public sealed class StageFieldProvider
    {
        private readonly IReadOnlyList<Tilemap> _biome;

        public StageFieldProvider(List<Tilemap> biome)
        {
            _biome = biome;
        }

        public Tilemap Get(int stageId)
        {
            // todo: stage를 Biome으로 묶기 
            var index = Math.Clamp(stageId - 1, 0, _biome.Count - 1);
            var biome = _biome[index];
            biome.CompressBounds();
            return biome;
        }
    }
}