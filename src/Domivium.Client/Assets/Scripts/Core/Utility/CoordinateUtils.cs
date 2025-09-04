using UnityEngine;
using UnityEngine.Tilemaps;

namespace Domivium.Client.Core.Utility
{
    public static class CoordinateUtils
    {
        private static readonly Plane GroundPlane = new(Vector3.up, Vector3.zero);

        public static bool TryScreenToCell(Camera camera, Tilemap grid, Vector2 screenPosition, out Vector3Int cell)
        {
            cell = default;
            if (camera == null || grid == null) return false;

            var ray = camera.ScreenPointToRay(screenPosition);
            if (!GroundPlane.Raycast(ray, out var hit)) return false;

            var world = ray.GetPoint(hit);
            cell = grid.WorldToCell(world);
            cell.z = 0;
            return true;
        }

        public static bool TryScreenToWorld(Camera camera, Vector2 screenPosition, out Vector3 worldPosition)
        {
            worldPosition = Vector3.zero;
            if (camera == null) return false;

            var ray = camera.ScreenPointToRay(screenPosition);
            if (!GroundPlane.Raycast(ray, out var hit)) return false;

            worldPosition = ray.GetPoint(hit);
            return true;
        }

        public static bool TryScreenToCollider(Camera camera, Vector2 screenPosition, out Collider collider)
        {
            collider = null;
            if (camera == null) return false;

            var ray = camera.ScreenPointToRay(screenPosition);
            if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, Layer.CharacterMask)) return false;

            collider = hit.collider;
            return true;
        }
    }
}