using UnityEngine;

namespace Domivium.Client.Contents.ReadModels
{
    public interface ICameraReadModel
    {
        public Camera MainCamera { get; }
        public Camera UICamera { get; }
        public bool TryScreenToWorld(Vector2 screenPosition, out Vector3 worldPosition);
        public bool TryScreenToCollider(Vector2 screenPosition, out Collider collider);
    }
}