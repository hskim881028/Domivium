using System.Collections.Generic;
using UnityEngine;

namespace Domivium.Client.Data.Store
{
    public interface IStageMapStore
    {
        void Initialize(HashSet<Vector3Int> cells);
        bool IsOccupied(Vector3Int cell);
        void Occupy(IEnumerable<Vector3Int> cells);
        void Release(IEnumerable<Vector3Int> cells);
    }
}