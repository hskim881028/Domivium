using System.Collections.Generic;
using UnityEngine;

namespace Domivium.Client.Data.Store
{
    public sealed class StageMapStore : IStageMapStore
    {
        private readonly HashSet<Vector3Int> _occupied = new();

        private BoundsInt _mapBounds;
        private Vector2Int _size;

        public void Initialize(BoundsInt mapBounds)
        {
            Reset();
            _mapBounds = mapBounds;
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

        public void SetSize(Vector2Int size) => _size = size;

        public void Reset()
        {
            _occupied.Clear();
            _mapBounds = new BoundsInt(Vector3Int.zero, Vector3Int.one);
            _size = Vector2Int.zero;
        }

        public void GetTower(Vector3Int pivot, IDictionary<Vector3Int, bool> buffer)
        {
            if (_size == Vector2Int.zero) return;

            buffer.Clear();
            for (var y = 0; y < _size.y; y++)
            for (var x = 0; x < _size.x; x++)
            {
                var cell = new Vector3Int(pivot.x + x, pivot.y + y, 0);
                buffer[cell] = CanPlace(cell);
            }
        }

        private bool CanPlace(Vector3Int cell)
        {
            if (!_mapBounds.Contains(cell)) return false;

            return !_occupied.Contains(cell);
        }
    }
}