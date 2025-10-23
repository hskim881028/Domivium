using System;
using System.Collections.Generic;
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
using Domivium.Client.Data.Info;
using Domivium.Client.Data.StageField;
using MessagePipe;
using R3;
using UnityEngine;

namespace Domivium.Client.Contents.Services
{
    public sealed class TowerPlacementService : Disposable, ITowerPlacementReadModel, ITowerPlacementCommand
    {
        private readonly MasterDbService _masterDbService;
        private readonly CoordinateService _coordinateService;
        private readonly StageFieldProvider _stageFieldProvider;
        private readonly IStageDirector _director;
        private readonly IActorSpawner _actorSpawner;
        private readonly IActorFactory _actorFactory;
        private readonly IStageInventoryReadModel _inventoryReadModel;
        private readonly IStageInventoryCommand _inventoryCommand;
        private readonly ReactiveProperty<bool> _ready;
        private readonly ReactiveProperty<StageCellInfo> _previewTower;
        private int _selectedSlot;

        public ReadOnlyReactiveProperty<bool> Ready => _ready;
        public ReadOnlyReactiveProperty<StageCellInfo> PreviewTower => _previewTower;

        public TowerPlacementService(
            MasterDbService masterDbService,
            CoordinateService coordinateService,
            StageFieldProvider stageFieldProvider,
            IStageDirector director,
            IActorSpawner actorSpawner,
            IActorFactory actorFactory,
            IStageInventoryReadModel inventoryReadModel,
            IStageInventoryCommand inventoryCommand,
            ISubscriber<SceneMessage> sceneSubscriber)
        {
            _masterDbService = masterDbService;
            _coordinateService = coordinateService;
            _stageFieldProvider = stageFieldProvider;
            _director = director;
            _actorSpawner = actorSpawner;
            _actorFactory = actorFactory;
            _inventoryReadModel = inventoryReadModel;
            _inventoryCommand = inventoryCommand;
            _ready = new ReactiveProperty<bool>().AddTo(ref DisposableBag);
            _previewTower = new ReactiveProperty<StageCellInfo>().AddTo(ref DisposableBag);
            sceneSubscriber.Subscribe(OnSceneMessage).AddTo(ref DisposableBag);
        }

        public async UniTask InitializeAsync(int stageId)
        {
            var biome = _stageFieldProvider.Get(stageId);
            await _actorSpawner.SpawnAsync(ActorIds.Map, new StageFieldParams(biome.cellBounds));

            var cells = new List<Vector3Int>();
            var stageRow = _masterDbService.DB.StageRowTable.FindByStageId(stageId);
            foreach (var row in stageRow)
            {
                var actorId = row.CampType.FromCampTypeToActorId();
                if (actorId == ActorIds.CharacterCamp) continue;

                var campIndex = row.CampIndex;
                var cell = new Vector3Int(row.X, row.Y, 0);
                _inventoryCommand.Restrict(cell);

                _stageFieldProvider.GetNeighbors(stageId, cell, cells, true);
                foreach (var c in cells)
                {
                    _inventoryCommand.Restrict(c);
                }

                if (actorId == ActorIds.Nexus)
                {
                    await _actorSpawner.SpawnAsync(actorId, _actorFactory.CreateNexus(1, cell));
                }

                if (actorId == ActorIds.MonsterCamp)
                {
                    await _actorSpawner.SpawnAsync(actorId, _actorFactory.CreateMonsterCamp(campIndex, cell));
                }
            }

            _ready.Value = true;
        }

        public void Show(int index)
        {
            _selectedSlot = index;
            _director.TrySetMode(StageModes.TowerPlacement);
        }

        public bool Hide()
        {
            ResetReadModel();
            return _director.TrySetMode(StageModes.Battle);
        }

        public bool Update(Vector2 position)
        {
            _previewTower.Value = _coordinateService.TryScreenToCell(position, out var cell)
                ? StageCellInfo.Create(cell, _inventoryReadModel.GetCellTag(cell, _selectedSlot))
                : StageCellInfo.Empty;
            return true;
        }

        public void Placement(Vector2 position)
        {
            Update(position);

            if (_previewTower.CurrentValue.Tag == StageCellTag.Occupiable)
            {
                var towerId = _inventoryReadModel.TowerSlot[_selectedSlot].TowerId;
                var param = _actorFactory.CreateTower(towerId, _previewTower.CurrentValue.Cell);
                _inventoryCommand.BuildTower(_selectedSlot);
                _actorSpawner.SpawnAsync(ActorIds.Tower, param).Forget();
            }

            if (_previewTower.CurrentValue.Tag == StageCellTag.Upgradeable)
            {
                _inventoryCommand.UpgradeTower(_selectedSlot, _previewTower.CurrentValue.Cell);
            }

            Hide();
        }

        private void ResetReadModel()
        {
            _previewTower.Value = StageCellInfo.Empty;
            _ready.Value = false;
        }

        private void OnSceneMessage(SceneMessage message)
        {
            switch (message.Type)
            {
                case SceneMessageType.Unload:
                case SceneMessageType.Load:
                    ResetReadModel();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}