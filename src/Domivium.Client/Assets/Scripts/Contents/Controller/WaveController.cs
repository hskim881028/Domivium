using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Actors.Generated;
using Domivium.Client.Contents.Context;
using Domivium.Client.Contents.Services;
using Domivium.Client.Contents.State;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Context;
using Domivium.Client.Core.Factory;
using Domivium.Client.Core.Message;
using Domivium.Client.Data.Row;
using MessagePipe;
using R3;
using UnityEngine;

namespace Domivium.Client.Contents.Controller
{
    public class WaveController : Disposable, IWaveController
    {
        private readonly StageContext _stageContext;
        private readonly MasterDbService _masterDbService;
        private readonly IActorSpawner _actorSpawner;
        private readonly IActorFactory _actorFactory;

        private readonly Dictionary<int, List<WaveRow>> _waves = new();
        private readonly Dictionary<int, Vector3Int> _camps = new();
        private readonly Queue<WaveRow> _pendingRemoves = new();
        private readonly List<WaveRow> _currentWaves = new();

        private readonly ReactiveProperty<int> _remainCount = new();
        private float _timer;
        private int _waveId;

        public WaveController(
            StageContext stageContext,
            MasterDbService masterDbService,
            IActorSpawner actorSpawner,
            IActorFactory actorFactory,
            ISubscriber<ActorStateMessage> actorTagSubscriber,
            ISubscriber<SceneMessage> sceneSubscriber)
        {
            _stageContext = stageContext;
            _masterDbService = masterDbService;
            _actorSpawner = actorSpawner;
            _actorFactory = actorFactory;

            stageContext.Phase.Subscribe(OnChangedPhase).AddTo(ref DisposableBag);
            actorTagSubscriber.Subscribe(OnActorStateMessage).AddTo(ref DisposableBag);
            sceneSubscriber.Subscribe(OnSceneMessage).AddTo(ref DisposableBag);
            _remainCount.Subscribe(OnUpdateRemainCount).AddTo(ref DisposableBag);
        }

        public void Initialize(int stageId)
        {
            var stageRow = _masterDbService.DB.StageRowTable.FindByStageId(stageId);

            foreach (var row in stageRow)
            {
                if (row.CampType.FromCampTypeToActorId() == ActorIds.MonsterCamp)
                {
                    _camps.Add(row.CampIndex, new Vector3Int(row.X, row.Y));
                }
            }

            var waveRows = _masterDbService.DB.WaveRowTable.FindByStageId(stageId);
            foreach (var row in waveRows)
            {
                if (!_waves.ContainsKey(row.WaveId))
                {
                    _waves[row.WaveId] = new List<WaveRow>();
                }

                _waves[row.WaveId].Add(row);
            }
        }

        public void Tick(float deltaTime)
        {
            if (_stageContext.Phase.CurrentValue != StagePhases.RunningWave) return;

            _timer += deltaTime;
            if (_currentWaves.Count > 0)
            {
                foreach (var wave in _currentWaves)
                {
                    if (wave.SpawnTime > _timer) continue;

                    SpawnAsync(wave.MonsterId, _camps[wave.CampIndex]).Forget();
                    _pendingRemoves.Enqueue(wave);
                }
            }

            while (_pendingRemoves.Count > 0)
            {
                var wave = _pendingRemoves.Dequeue();
                _currentWaves.Remove(wave);
            }
        }

        private async UniTaskVoid SpawnAsync(int monsterId, Vector3Int spawnPoint)
        {
            var monster = _actorFactory.CreateMonster(monsterId, spawnPoint);
            await _actorSpawner.SpawnAsync(ActorIds.Monster, monster);
        }

        private void OnUpdateRemainCount(int count)
        {
            this.Log($"[{_waveId} Wave] RemainCount: {_remainCount.CurrentValue}");
            if (_stageContext.Phase.CurrentValue != StagePhases.RunningWave) return;

            if (_remainCount.CurrentValue > 0) return;

            NextWave();
        }

        private void NextWave()
        {
            _waveId++;
            if (!_waves.TryGetValue(_waveId, out var waves))
            {
                this.Error($"Wave not found: {_waveId}");
                return;
            }

            _remainCount.Value = waves.Count;
            _timer = 0;
            _pendingRemoves.Clear();
            _currentWaves.Clear();
            _currentWaves.AddRange(waves);
        }

        private void OnActorStateMessage(ActorStateMessage message)
        {
            if (message.ActorId == ActorIds.Monster && message.Tag == StateTags.Die)
            {
                _remainCount.Value--;
            }
        }

        private void OnChangedPhase(StagePhase phase)
        {
            if (phase == StagePhases.RunningWave)
            {
                NextWave();
            }

            if (phase == StagePhases.Failed || phase == StagePhases.Cleared) { }
        }

        private void OnSceneMessage(SceneMessage message)
        {
            switch (message.Type)
            {
                case SceneMessageType.Unload:
                case SceneMessageType.Load:
                    _remainCount.Value = 0;
                    _timer = 0;
                    _waveId = 0;
                    _waves.Clear();
                    _camps.Clear();
                    _pendingRemoves.Clear();
                    _currentWaves.Clear();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}