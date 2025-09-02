using System.Collections.Generic;
using UnityEngine;

namespace Domivium.Client.Data.Store
{
    public interface IStageMapStore
    {
        public void Initialize(BoundsInt mapBounds);
        public void Occupy(IEnumerable<Vector3Int> cells);
        public void Release(IEnumerable<Vector3Int> cells);
        public void Select(Vector2Int size);
        public void Reset();
        public void GetTower(Vector3Int pivot, IDictionary<Vector3Int, bool> buffer);
    }
}