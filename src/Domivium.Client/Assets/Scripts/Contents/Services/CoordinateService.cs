using Domivium.Client.Contents.Actors;
using Domivium.Client.Contents.ReadModels;
using Domivium.Client.Core.Message;
using MessagePipe;
using R3;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Domivium.Client.Contents.Services
{
    public sealed class CoordinateService : Disposable
    {
        private readonly ICameraReadModel _cameraRead;
        private Tilemap _grid;

        public CoordinateService(
            ICameraReadModel cameraRead,
            ISubscriber<SpawnActorMessage> spawnerSubscriber)
        {
            _cameraRead = cameraRead;
            spawnerSubscriber.Subscribe(OnSpawnerMessage).AddTo(ref DisposableBag);
        }

        public Vector3 GetWorldPosition(Vector3Int cell) => _grid.GetCellCenterWorld(cell);

        public Vector3Int GetCellPoint(Vector3 worldPosition) => _grid.WorldToCell(worldPosition);

        public bool TryScreenToCell(Vector2 screenPosition, out Vector3Int cell)
        {
            cell = default;
            if (!_cameraRead.TryScreenToWorld(screenPosition, out var worldPosition)) return false;

            cell = _grid.WorldToCell(worldPosition);
            cell.z = 0;
            return true;
        }

        public bool TryScreenToWorld(Vector2 screenPosition, out Vector3 worldPosition)
        {
            return _cameraRead.TryScreenToWorld(screenPosition, out worldPosition);
        }

        public bool TryScreenToCollider(Vector2 screenPosition, out Collider collider)
        {
            return _cameraRead.TryScreenToCollider(screenPosition, out collider);
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