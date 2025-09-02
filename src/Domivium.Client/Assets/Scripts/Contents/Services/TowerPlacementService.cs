using System;
using System.Collections.Generic;
using Domivium.Client.Contents.Commands;
using Domivium.Client.Contents.Context;
using Domivium.Client.Contents.ReadModels;
using Domivium.Client.Core.Director;
using Domivium.Client.Core.Message;
using Domivium.Client.Core.Provider;
using Domivium.Client.Core.Utility;
using Domivium.Client.Data.Store;
using MessagePipe;
using ObservableCollections;
using R3;
using UnityEngine;
using UnityEngine.Tilemaps;
using DisposableBag = R3.DisposableBag;

namespace Domivium.Client.Contents.Services
{
    public sealed class TowerPlacementService : ITowerPlacementReadModel, ITowerPlacementCommand, IDisposable
    {
        private readonly IStageDirector _director;
        private readonly IStageMapStore _store;
        private readonly ICameraReadModel _cameraRead;
        private readonly StageMapProvider _stageMapProvider;
        private readonly ObservableList<Vector3Int> _stagedTower = new();
        private readonly ObservableDictionary<Vector3Int, bool> _tower = new();

        private Plane _groundPlane = new(Vector3.up, Vector3.zero);
        private Tilemap _grid;
        private DisposableBag _disposable;
        private bool _isDisposed;

        public IReadOnlyObservableList<Vector3Int> StagedTower => _stagedTower;

        public IReadOnlyObservableDictionary<Vector3Int, bool> PreviewTower => _tower;

        public TowerPlacementService(
            StageMapProvider stageMapProvider,
            IStageDirector director,
            IStageMapStore store,
            ICameraReadModel cameraRead,
            ISubscriber<SceneMessage> subscriber)
        {
            _stageMapProvider = stageMapProvider;
            _director = director;
            _store = store;
            _cameraRead = cameraRead;
            subscriber.Subscribe(OnSceneMessage).AddTo(ref _disposable);
        }

        public IReadOnlyCollection<Vector3Int> Initialize(int stageId)
        {
            var tilemap = _stageMapProvider.Get(stageId);
            var cells = new HashSet<Vector3Int>();
            foreach (var cell in tilemap.cellBounds.allPositionsWithin)
            {
                cells.Add(cell);
            }

            _store.Initialize(tilemap.cellBounds);
            return cells;
        }

        public void SetGrid(Tilemap tilemap)
        {
            _grid = tilemap;
        }

        public void Show(int index)
        {
            var size = index switch
            {
                1 => new Vector2Int(1, 2),
                2 => new Vector2Int(2, 1),
                _ => new Vector2Int(1, 1)
            };

            _store.Select(size);
            _director.TrySetMode(StageModes.TowerPlacement);
        }

        public bool Hide()
        {
            ResetReadModel();
            return _director.TrySetMode(StageModes.Idle);
        }

        public bool Update(Vector2 position)
        {
            var ray = _cameraRead.MainCamera.ScreenPointToRay(position);
            if (!_groundPlane.Raycast(ray, out var hit)) return true;

            var worldPosition = ray.GetPoint(hit);
            var pivot = _grid.WorldToCell(worldPosition);
            pivot.z = 0;
            _tower.Clear();
            _store.GetTower(pivot, _tower);
            return true;
        }

        public bool Placement(Vector2 position)
        {
            Update(position);

            foreach (var (_, canPlace) in _tower)
            {
                if (!canPlace) return false;
            }

            _stagedTower.Clear();
            foreach (var (cell, _) in _tower)
            {
                _stagedTower.Add(cell);
            }

            _store.Occupy(_stagedTower);
            Hide();
            return true;
        }

        public void Dispose()
        {
            if (_isDisposed) return;

            _isDisposed = true;
            _disposable.Dispose();
        }

        private void ResetReadModel()
        {
            _stagedTower.Clear();
            _tower.Clear();
        }

        private void OnSceneMessage(SceneMessage message)
        {
            switch (message.Type)
            {
                case SceneMessageType.Unload:
                case SceneMessageType.Load:
                    ResetReadModel();
                    _store.Reset();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}