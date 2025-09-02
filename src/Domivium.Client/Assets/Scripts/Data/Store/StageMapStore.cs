using System.Collections.Generic;
using UnityEngine;

namespace Domivium.Client.Data.Store
{
    public sealed class StageMapStore : IStageMapStore
    {
        private readonly HashSet<Vector3Int> _all = new();
        private readonly HashSet<Vector3Int> _occupied = new();

        private BoundsInt _mapBounds;
        private Vector2Int _size;

        public void Initialize(BoundsInt mapBounds)
        {
            Reset();
            _mapBounds = mapBounds;
            foreach (var cell in _mapBounds.allPositionsWithin)
            {
                _all.Add(cell);
            }
        }

        public void Occupy(IEnumerable<Vector3Int> cells)
        {
            foreach (var c in cells)
            {
                _occupied.Add(c);
            }
        }

        public void Release(IEnumerable<Vector3Int> cells)
        {
            foreach (var c in cells)
            {
                _occupied.Remove(c);
            }
        }

        public void Select(Vector2Int size)
        {
            _size = size;
        }

        public void Reset()
        {
            _all.Clear();
            _occupied.Clear();
            _mapBounds = new BoundsInt(Vector3Int.zero, Vector3Int.one);
            _size = Vector2Int.zero;
        }

        public void GetTower(Vector3Int pivot, IDictionary<Vector3Int, bool> buffer)
        {
            buffer.Clear();
            var expected = _size.x * _size.y;
            if (buffer is Dictionary<Vector3Int, bool> dic)
            {
                dic.EnsureCapacity(expected);
            }

            for (var y = 0; y < _size.y; y++)
            {
                for (var x = 0; x < _size.x; x++)
                {
                    var cell = new Vector3Int(pivot.x + x, pivot.y + y, 0);
                    buffer[cell] = CanPlace(cell);
                }
            }
        }


        public IReadOnlyDictionary<Vector3Int, bool> GetTower(Vector3Int pivot)
        {
            var cells = new Dictionary<Vector3Int, bool>();
            for (var y = 0; y < _size.y; y++)
            {
                for (var x = 0; x < _size.x; x++)
                {
                    var cell = new Vector3Int(pivot.x + x, pivot.y + y, 0);
                    var canPlace = CanPlace(cell);
                    cells.Add(cell, canPlace);
                }
            }
            return cells;
        }

        private bool CanPlace(Vector3Int cell)
        {
            if (!_mapBounds.Contains(cell)) return false;

            if (!_all.Contains(cell)) return false;

            return !_occupied.Contains(cell);
        }
    }
}