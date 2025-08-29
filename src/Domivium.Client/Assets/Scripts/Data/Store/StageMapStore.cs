using System.Collections.Generic;
using UnityEngine;

namespace Domivium.Client.Data.Store
{
    public class StageMapStore : IStageMapStore
    {
        private IReadOnlyCollection<Vector3Int> _all;
        private readonly HashSet<Vector3Int> _occupied = new();

        public void Initialize(HashSet<Vector3Int> cells)
        {
            _all = cells;
        }

        public bool IsOccupied(Vector3Int cell) => _occupied.Contains(cell);

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
    }
}