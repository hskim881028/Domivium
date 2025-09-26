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
        private readonly MasterDbService _masterDbService;
        private readonly CoordinateService _coordinateService;
        private readonly StageMapProvider _stageMapProvider;
        private readonly IStageDirector _director;
        private readonly IActorSpawner _actorSpawner;
        private readonly IActorFactory _actorFactory;
        private readonly IStageMapStore _store;
        private readonly ObservableList<Vector3Int> _stagedTower = new();
        private readonly ObservableDictionary<Vector3Int, bool> _previewTower = new();
        private readonly ReactiveProperty<bool> _ready;

        public IReadOnlyObservableList<Vector3Int> StagedTower => _stagedTower;
        public IReadOnlyObservableDictionary<Vector3Int, bool> PreviewTower => _previewTower;
        public ReadOnlyReactiveProperty<bool> Ready => _ready;

        public TowerPlacementService(
            MasterDbService masterDbService,
            CoordinateService coordinateService,
            StageMapProvider stageMapProvider,
            IStageDirector director,
            IActorSpawner actorSpawner,
            IActorFactory actorFactory,
            IStageMapStore store,
            ISubscriber<SceneMessage> sceneSubscriber)
        {
            _masterDbService = masterDbService;
            _coordinateService = coordinateService;
            _stageMapProvider = stageMapProvider;
            _director = director;
            _actorSpawner = actorSpawner;
            _actorFactory = actorFactory;
            _store = store;
            _ready = new ReactiveProperty<bool>().AddTo(ref DisposableBag);
            sceneSubscriber.Subscribe(OnSceneMessage).AddTo(ref DisposableBag);
        }

        public async UniTask InitializeAsync(int stageId)
        {
            var biome = _stageMapProvider.Get(stageId);
            _store.Initialize(biome.cellBounds);

            await _actorSpawner.SpawnAsync(ActorIds.Map, new StageMapParams(biome.cellBounds));

            var cells = new List<Vector3Int>();
            var stageRow = _masterDbService.DB.StageRowTable.FindByStageId(stageId);
            foreach (var row in stageRow)
            {
                var actorId = row.CampType.FromCampTypeToActorId();
                var campIndex = row.CampIndex;
                var cell = new Vector3Int(row.X, row.Y, 0);
                _store.GetNeighbors(cell, cells);
                _stagedTower.Clear();
                foreach (var c in cells)
                {
                    _stagedTower.Add(c);
                }

                _store.Occupy(_stagedTower, cell);

                var param = _actorFactory.CreateCamp(stageId, actorId, campIndex, cell);
                await _actorSpawner.SpawnAsync(actorId, param);
            }

            _ready.Value = true;
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

                var tower = _stagedTower.First();
                _store.Occupy(_stagedTower, tower);
                var param = _actorFactory.CreateTower(1, tower);
                _actorSpawner.SpawnAsync(ActorIds.Tower, param).Forget();
            }

            Hide();
            return true;
        }

        private void ResetReadModel()
        {
            _stagedTower.Clear();
            _previewTower.Clear();
            _ready.Value = false;
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