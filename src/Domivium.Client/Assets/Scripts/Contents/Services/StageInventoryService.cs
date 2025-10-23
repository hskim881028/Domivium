using System;
using Domivium.Client.Contents.Actors.Generated;
using Domivium.Client.Contents.Battle;
using Domivium.Client.Contents.Commands;
using Domivium.Client.Contents.ReadModels;
using Domivium.Client.Contents.State;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Message;
using Domivium.Client.Core.Provider;
using Domivium.Client.Data.Info;
using Domivium.Client.Data.StageField;
using MessagePipe;
using ObservableCollections;
using R3;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Domivium.Client.Contents.Services
{
    public sealed class StageInventoryService : Disposable, IStageInventoryReadModel, IStageInventoryCommand
    {
        private readonly MasterDbService _masterDbService;
        private readonly CoordinateService _coordinateService;
        private readonly StageFieldProvider _stageFieldProvider;
        private readonly IActorManager _actorManager;
        private readonly IBattleEffectPool _effectPool;
        private readonly IPublisher<BattleCueMessage> _cuePublisher;
        private readonly ReactiveProperty<int> _rerollCost = new();
        private readonly ReactiveProperty<int> _soul = new();
        private readonly ReactiveProperty<int> _towerLimit = new();
        private readonly ObservableDictionary<int, TowerSlotInfo> _towerSlot = new();
        private readonly ObservableDictionary<Vector3Int, IBattleSystem> _towers = new();
        private readonly ObservableList<Vector3Int> _barrier = new();
        private BoundsInt _groundBounds;

        public ReadOnlyReactiveProperty<int> RerollCost => _rerollCost;
        public ReadOnlyReactiveProperty<int> Soul => _soul;
        public ReadOnlyReactiveProperty<int> TowerLimit => _towerLimit;
        public IReadOnlyObservableDictionary<int, TowerSlotInfo> TowerSlot => _towerSlot;
        public IReadOnlyObservableDictionary<Vector3Int, IBattleSystem> Towers => _towers;
        public IReadOnlyObservableList<Vector3Int> Barrier => _barrier;

        public StageInventoryService(
            MasterDbService masterDbService,
            CoordinateService coordinateService,
            StageFieldProvider stageFieldProvider,
            IActorManager actorManager,
            IBattleEffectPool effectPool,
            IPublisher<BattleCueMessage> cuePublisher,
            ISubscriber<ActorStateMessage> actorStateSubscriber,
            ISubscriber<SpawnActorMessage> spawnerSubscriber,
            ISubscriber<SceneMessage> sceneSubscriber)
        {
            _masterDbService = masterDbService;
            _coordinateService = coordinateService;
            _stageFieldProvider = stageFieldProvider;
            _actorManager = actorManager;
            _effectPool = effectPool;
            _cuePublisher = cuePublisher;
            actorStateSubscriber.Subscribe(OnActorStateMessage).AddTo(ref DisposableBag);
            spawnerSubscriber.Subscribe(OnSpawnerMessage).AddTo(ref DisposableBag);
            sceneSubscriber.Subscribe(OnSceneMessage).AddTo(ref DisposableBag);
        }

        public bool CanReroll() => _soul.CurrentValue >= _rerollCost.CurrentValue;

        public bool CanSelectTower(int slotIndex)
        {
            if (!_towerSlot.TryGetValue(slotIndex, out var info)) return false;
            
            if(_soul.Value < info.Cost) return false;
            
            if (_towers.Count < _towerLimit.CurrentValue) return true;
            
            foreach (var (_, tower) in _towers)
            {
                if (tower.Id == info.TowerId) return true;
            }
                
            return false;
        }

        public bool CanMove(Vector3Int cell)
        {
            if (!_groundBounds.Contains(cell)) return false;

            if (_barrier.Contains(cell)) return false;

            if (_towers.ContainsKey(cell)) return false;

            return true;
        }

        public StageCellTag GetCellTag(Vector3Int cell, int slotIndex)
        {
            if (_barrier.Contains(cell)) return StageCellTag.Blocked;
   
            if (!_towers.TryGetValue(cell, out var tower))
            {
                return _towers.Count < _towerLimit.CurrentValue ? StageCellTag.Occupiable : StageCellTag.Blocked;
            }
            
            return tower.Id == _towerSlot[slotIndex].TowerId ? StageCellTag.Upgradeable : StageCellTag.Blocked;
        }

        public void Initialize(int stageId, int startSoul, int rerollCost, int towerLimit)
        {
            var tilemap = _stageFieldProvider.Get(stageId);
            _groundBounds = tilemap.cellBounds;
            _soul.Value = startSoul;
            _rerollCost.Value = rerollCost;
            _towerLimit.Value = towerLimit;
            RefillTower(0);
        }

        public void RefillTower(int cost)
        {
            _soul.Value -= cost;
            for (var i = 0; i < 5; i++)
            {
                var id = Random.Range(1, 4);
                var row = _masterDbService.DB.TowerRowTable.FindById(id);
                // _towerSlot[i] = new TowerSlotInfo(row.Id, row.Cost);
                _towerSlot[i] = TowerSlotInfo.Create(row.Id, row.Id); // for test
            }
        }

        public void BuildTower(int slotIndex)
        {
            _towerSlot.TryGetValue(slotIndex, out var info);
            _soul.Value -= info.Cost;
            _towerSlot.Remove(slotIndex);
        }

        public void UpgradeTower(int slotIndex, Vector3Int cell)
        {
            _towerSlot.TryGetValue(slotIndex, out var info);
            _soul.Value -= info.Cost;
            _towerSlot.Remove(slotIndex);

            var tower = _towers[cell];
            var context = BattleAbilityContext.Create(BattleAbilityIds.Attack, tower, tower);
            var effectSpec = _effectPool.Get(BattleEffectIds.LevelUp, context);
            tower.ActivateEffect(effectSpec);
        }

        public void Restrict(Vector3Int cell)
        {
            _barrier.Add(cell);
        }

        private void AddSoul(ushort uid, int id)
        {
            if (!_actorManager.TryGetPawn(ActorIds.Monster, uid, out var monster)) return;

            if (!_masterDbService.DB.MonsterRowTable.TryFindById(id, out var row)) return;

            if (!_actorManager.TryGetFirstPawn(ActorIds.Nexus, out var nexus)) return;

            this.Log($"rarity : {row.Soul}");
            _soul.Value += row.Soul;
            var context = BattleCueContext.Create(monster.ActorId, monster.UnitPosition, nexus.UnitPosition);
            _cuePublisher.Publish(BattleCueMessage.Emit(BattleCueIds.DropSoul, context));
        }

        private void AddTower(ushort uid)
        {
            if (!_actorManager.TryGetPawn(ActorIds.Tower, uid, out var tower)) return;

            var cell = _coordinateService.GetCellPoint(tower.UnitPosition);
            _towers.Add(cell, tower);
        }

        private void RemoveTower(ushort uid)
        {
            Vector3Int cellToRemove = default;
            var found = false;

            foreach (var kv in _towers)
            {
                if (kv.Value.Uid != uid) continue;

                cellToRemove = kv.Key;
                found = true;
                break;
            }

            if (found)
            {
                _towers.Remove(cellToRemove);
            }
        }

        private void OnActorStateMessage(ActorStateMessage message)
        {
            if (message.ActorId == ActorIds.Monster && message.Tag == StateTags.Die)
            {
                AddSoul(message.Uid, message.Id);
            }

            if (message.ActorId == ActorIds.Tower && message.Tag == StateTags.Die)
            {
                RemoveTower(message.Uid);
            }
        }

        private void OnSpawnerMessage(SpawnActorMessage message)
        {
            if (message.ActorId == ActorIds.Tower)
            {
                AddTower(message.Uid);
            }
        }

        private void OnSceneMessage(SceneMessage message)
        {
            switch (message.Type)
            {
                case SceneMessageType.Unload:
                case SceneMessageType.Load:
                    _soul.Value = 0;
                    _towers.Clear();
                    _barrier.Clear();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}