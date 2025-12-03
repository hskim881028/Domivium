using System.Collections.Generic;
using UnityEngine;

namespace Domivium.Client.Core.Utility
{
    public sealed class HitDistanceComparer : IComparer<RaycastHit2D>
    {
        public static readonly HitDistanceComparer Instance = new();

        public int Compare(RaycastHit2D a, RaycastHit2D b)
            => a.distance.CompareTo(b.distance);
    }
}