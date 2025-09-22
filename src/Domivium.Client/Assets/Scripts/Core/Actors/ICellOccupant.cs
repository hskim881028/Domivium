using UnityEngine;

namespace Domivium.Client.Core.Actors
{
    public interface ICellOccupant
    {
        public Vector3Int Cell { get; }
    }
}