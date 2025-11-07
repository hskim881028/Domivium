using System;
using System.Collections.Generic;
using UnityEngine;
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
        
        public void GetNeighbors(
            int stageId,
            Vector3Int pivot,
            in IList<Vector3Int> buffer,
            bool excludeCenter = false,
            bool excludeDiagonal = false)
        {
            var groundBounds = Get(stageId).cellBounds;
            var gxMin = groundBounds.xMin;
            var gxMax = groundBounds.xMax - 1;
            var gyMin = groundBounds.yMin;
            var gyMax = groundBounds.yMax - 1;

            for (var y = pivot.y - 1; y <= pivot.y + 1; y++)
            {
                for (var x = pivot.x - 1; x <= pivot.x + 1; x++)
                {
                    if (excludeCenter && x == pivot.x && y == pivot.y) continue;

                    if (excludeDiagonal)
                    {
                        var dx = Math.Abs(x - pivot.x);
                        var dy = Math.Abs(y - pivot.y);
                        if (dx == 1 && dy == 1) continue;
                    }

                    if (x < gxMin || x > gxMax || y < gyMin || y > gyMax) continue;

                    buffer.Add(new Vector3Int(x, y, 0));
                }
            }
        }
    }
}