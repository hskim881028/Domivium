using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Actors.Contract;
using Domivium.Client.Contents.Actors.Generated;
using Domivium.Client.Contents.Commands;
using Domivium.Client.Contents.Context;
using Domivium.Client.Contents.ReadModels;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Director;
using Domivium.Client.Core.Factory;
using Domivium.Client.Core.Message;
using Domivium.Client.Core.Provider;
using Domivium.Client.Data.Store;
using MessagePipe;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Domivium.Client.Contents.Services
{
    public sealed class TowerPlacementService : Disposable, ITowerPlacementReadModel, ITowerPlacementCommand
    {
        private readonly CoordinateService _coordinateService;
        private readonly StageMapProvider _stageMapProvider;
        private readonly IStageDirector _director;
        private readonly IActorSpawner _actorSpawner;
        private readonly IUnitFactory _unitFactory;
        private readonly IStageMapStore _store;
        private readonly ObservableList<Vector3Int> _stagedTower = new();
        private readonly ObservableDictionary<Vector3Int, bool> _previewTower = new();

        public IReadOnlyObservableList<Vector3Int> StagedTower => _stagedTower;

        public IReadOnlyObservableDictionary<Vector3Int, bool> PreviewTower => _previewTower;

        public TowerPlacementService(
            CoordinateService coordinateService,
            StageMapProvider stageMapProvider,
            IStageDirector director,
            IActorSpawner actorSpawner,
            IUnitFactory unitFactory,
            IStageMapStore store,
            ISubscriber<SceneMessage> sceneSubscriber)
        {
            _coordinateService = coordinateService;
            _stageMapProvider = stageMapProvider;
            _director = director;
            _actorSpawner = actorSpawner;
            _unitFactory = unitFactory;
            _store = store;
            sceneSubscriber.Subscribe(OnSceneMessage).AddTo(ref DisposableBag);
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
            if (_coordinateService.TryScreenToCell(position, out var cell))
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
                var param = _unitFactory.CreateTower(1, _stagedTower.First());
                _actorSpawner.SpawnAsync(ActorIds.Tower, param).Forget();
            }

            Hide();
            return true;
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
    }
}