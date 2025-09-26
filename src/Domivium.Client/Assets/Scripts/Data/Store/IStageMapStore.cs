using System.Collections.Generic;
using UnityEngine;

namespace Domivium.Client.Data.Store
{
    public interface IStageMapStore
    {
        public void Initialize(BoundsInt groundBounds);
        public void Occupy(IEnumerable<Vector3Int> cells, Vector3Int tower);
        public void Release(IEnumerable<Vector3Int> cells);
        public void SetSize(Vector2Int size);
        public void Reset();
        public void GetTower(Vector3Int pivot, in IDictionary<Vector3Int, bool> buffer);
        public void GetNeighbors(Vector3Int pivot, in IList<Vector3Int> buffer, bool excludeCenter = false, bool excludeDiagonal = false);
        public bool CanMove(Vector3Int cell);
    }
}