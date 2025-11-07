namespace Domivium.Client.Core.Utility
{
    public sealed class HitDistanceComparer : System.Collections.Generic.IComparer<UnityEngine.RaycastHit2D>
    {
        public static readonly HitDistanceComparer Instance = new();

        public int Compare(UnityEngine.RaycastHit2D a, UnityEngine.RaycastHit2D b)
            => a.distance.CompareTo(b.distance);
    }
}