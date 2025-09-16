using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Actors;
using Domivium.Client.Contents.Actors.Contract;
using Domivium.Client.Contents.Actors.Generated;
using Domivium.Client.Contents.Commands;
using Domivium.Client.Contents.Context;
using Domivium.Client.Contents.ReadModels;
using Domivium.Client.Core.Actors;
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
        private readonly StageMapProvider _stageMapProvider;
        private readonly IStageDirector _director;
        private readonly IActorSpawner _actorSpawner;
        private readonly IStageMapStore _store;
        private readonly ICameraReadModel _cameraRead;
        private readonly ObservableList<Vector3Int> _stagedTower = new();
        private readonly ObservableDictionary<Vector3Int, bool> _previewTower = new();

        private Tilemap _grid;
        private DisposableBag _disposable;
        private bool _isDisposed;

        public IReadOnlyObservableList<Vector3Int> StagedTower => _stagedTower;

        public IReadOnlyObservableDictionary<Vector3Int, bool> PreviewPreviewTower => _previewTower;

        public TowerPlacementService(
            StageMapProvider stageMapProvider,
            IStageDirector director,
            IActorSpawner actorSpawner,
            IStageMapStore store,
            ICameraReadModel cameraRead,
            ISubscriber<SceneMessage> sceneSubscriber,
            ISubscriber<SpawnerMessage> spawnerSubscriber)
        {
            _stageMapProvider = stageMapProvider;
            _director = director;
            _actorSpawner = actorSpawner;
            _store = store;
            _cameraRead = cameraRead;
            sceneSubscriber.Subscribe(OnSceneMessage).AddTo(ref _disposable);
            spawnerSubscriber.Subscribe(OnSpawnerMessage).AddTo(ref _disposable);
        }

        public async UniTask InitializeAsync(int stageId)
        {
            var tilemap = _stageMapProvider.Get(stageId);
            var cells = new HashSet<Vector3Int>();
            foreach (var cell in tilemap.cellBounds.allPositionsWithin)
            {
                cells.Add(cell);
            }

            _store.Initialize(tilemap.cellBounds);
            await _actorSpawner.SpawnAsync(ActorIds.Map, new ActorParams(cells));
        }

        public void SetGrid(Tilemap tilemap) => _grid = tilemap;

        public void Show(int index)
        {
            var size = index switch
            {
                1 => new Vector2Int(1, 2),
                2 => new Vector2Int(2, 1),
                _ => new Vector2Int(1, 1)
            };

            _store.SetSize(size);
            _director.TrySetMode(StageModes.TowerPlacement);
        }

        public bool Hide()
        {
            ResetReadModel();
            return _director.TrySetMode(StageModes.Battle);
        }

        public bool Update(Vector2 position)
        {
            _previewTower.Clear();
            if (CoordinateUtils.TryScreenToCell(_cameraRead.MainCamera, _grid, position, out var cell))
            {
                _store.GetTower(cell, _previewTower);
            }

            return true;
        }

        public bool Placement(Vector2 position)
        {
            Update(position);

            if (_previewTower.All(x => x.Value))
            {
                _stagedTower.Clear();
                foreach (var (cell, _) in _previewTower)
                {
                    _stagedTower.Add(cell);
                }

                _store.Occupy(_stagedTower);
                _actorSpawner.SpawnAsync(ActorIds.Tower, new TowerParams(_stagedTower.First())).Forget();
            }

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
            _previewTower.Clear();
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

        private void OnSpawnerMessage(SpawnerMessage message)
        {
            switch (message.Type)
            {
                case SpawnerMessageType.Spawn:
                    if (message.Presenter is StageMapPresenter stageMapPresenter)
                    {
                        _grid = stageMapPresenter.Grid;
                    }

                    break;
                case SpawnerMessageType.Despawn:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}