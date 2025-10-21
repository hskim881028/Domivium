using System;
using System.Collections.Generic;
using UnityEngine;

namespace Domivium.Client.Data.Store
{
    public sealed class StageFieldStore : IStageFieldStore
    {
        private readonly HashSet<Vector3Int> _occupied = new();
        private readonly HashSet<Vector3Int> _tower = new();

        private BoundsInt _groundBounds;
        private Vector2Int _size;

        public void Initialize(BoundsInt groundBounds)
        {
            Reset();
            _groundBounds = groundBounds;
        }

        public bool CanMove(Vector3Int cell)
        {
            if (!_groundBounds.Contains(cell)) return false;

            return !_tower.Contains(cell);
        }

        public void Occupy(IEnumerable<Vector3Int> cells, Vector3Int tower)
        {
            foreach (var c in cells)
            {
                _occupied.Add(c);
            }

            _tower.Add(tower);
        }

        public void Release(IEnumerable<Vector3Int> cells)
        {
            foreach (var c in cells)
            {
                _occupied.Remove(c);
                _tower.Remove(c);
            }
        }

        public void SetSize(Vector2Int size) => _size = size;

        public void Reset()
        {
            _occupied.Clear();
            _tower.Clear();
            _size = Vector2Int.zero;
        }

        public void GetTower(Vector3Int pivot, in IDictionary<Vector3Int, bool> buffer)
        {
            if (_size.x <= 0 || _size.y <= 0) return;

            buffer.Clear();
            var gxMin = _groundBounds.xMin;
            var gxMax = _groundBounds.xMax - 1;
            var gyMin = _groundBounds.yMin;
            var gyMax = _groundBounds.yMax - 1;

            for (var dy = 0; dy < _size.y; dy++)
            {
                var cy = pivot.y + dy;
                var yOut = cy < gyMin || cy > gyMax;

                for (var dx = 0; dx < _size.x; dx++)
                {
                    var cx = pivot.x + dx;
                    var cell = new Vector3Int(cx, cy, 0);
                    if (yOut || cx < gxMin || cx > gxMax)
                    {
                        buffer[cell] = false;
                        continue;
                    }
                    buffer[cell] = !_occupied.Contains(cell);
                }
            }
        }

        public void GetNeighbors(
            Vector3Int pivot,
            in IList<Vector3Int> buffer,
            bool excludeCenter = false,
            bool excludeDiagonal = false)
        {
            buffer.Clear();

            var gxMin = _groundBounds.xMin;
            var gxMax = _groundBounds.xMax - 1;
            var gyMin = _groundBounds.yMin;
            var gyMax = _groundBounds.yMax - 1;

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

                    var cell = new Vector3Int(x, y, 0);
                    if (_occupied.Contains(cell)) continue;

                    buffer.Add(cell);
                }
            }
        }
    }
}