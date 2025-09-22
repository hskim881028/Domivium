using Domivium.Client.Contents.Actors;
using Domivium.Client.Contents.ReadModels;
using Domivium.Client.Core.Message;
using Domivium.Client.Core.Utility;
using MessagePipe;
using R3;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Domivium.Client.Contents.Services
{
    public sealed class CoordinateService : Disposable
    {
        private readonly Plane _groundPlane = new(Vector3.up, Vector3.zero);
        private readonly ICameraReadModel _cameraRead;
        private Tilemap _grid;

        public CoordinateService(
            ICameraReadModel cameraRead,
            ISubscriber<SpawnActorMessage> spawnerSubscriber)
        {
            _cameraRead = cameraRead;
            spawnerSubscriber.Subscribe(OnSpawnerMessage).AddTo(ref DisposableBag);
        }

        public Vector3Int GetPosition(Vector3 worldPosition)
        {
            return _grid.WorldToCell(worldPosition);
        }

        public bool TryScreenToCell(Vector2 screenPosition, out Vector3Int cell)
        {
            cell = default;
            var ray = _cameraRead.MainCamera.ScreenPointToRay(screenPosition);
            if (!_groundPlane.Raycast(ray, out var hit)) return false;

            var world = ray.GetPoint(hit);
            cell = _grid.WorldToCell(world);
            cell.z = 0;
            return true;
        }

        public bool TryScreenToWorld(Vector2 screenPosition, out Vector3 worldPosition)
        {
            worldPosition = Vector3.zero;
            var ray = _cameraRead.MainCamera.ScreenPointToRay(screenPosition);
            if (!_groundPlane.Raycast(ray, out var hit)) return false;

            worldPosition = ray.GetPoint(hit);
            return true;
        }

        public bool TryScreenToCollider(Vector2 screenPosition, out Collider collider)
        {
            collider = null;

            var ray = _cameraRead.MainCamera.ScreenPointToRay(screenPosition);
            if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, Layer.CharacterMask)) return false;

            collider = hit.collider;
            return true;
        }

        private void OnSpawnerMessage(SpawnActorMessage message)
        {
            if (message.Presenter is StageMapPresenter stageMapPresenter)
            {
                _grid = stageMapPresenter.Grid;
            }
        }
    }
}