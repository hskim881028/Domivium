using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Actors.Generated;
using Domivium.Client.Contents.Audio.Generated;
using Domivium.Client.Contents.Commands;
using Domivium.Client.Contents.Context;
using Domivium.Client.Contents.Controller;
using Domivium.Client.Contents.ReadModels;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Audio;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Director;
using Domivium.Client.Core.Input;
using Domivium.Client.Core.Message;
using Domivium.Client.Core.Provider;
using Domivium.Client.Core.State;
using Domivium.Client.Data.Config;
using MessagePipe;
using R3;
using UnityEngine;
using VContainer.Unity;

namespace Domivium.Client.Contents.DI.Entry
{
    public class StageEntry : Entry, ITickable
    {
        private readonly StageMapProvider _stageMapProvider;
        private readonly IStageDirector _stageDirector;
        private readonly IWaveController _waveController;
        private readonly IBattleService _battleService;
        private readonly ICameraCommand _cameraCommand;
        private readonly ITowerPlacementCommand _towerPlacementCommand;
        private readonly IBattleUserCommand _battleUserCommand;
        private readonly IActorManager _actorManager;

        public StageEntry(
            IInputComposition inputComposition,
            IBattleCuePlayer cuePlayer,
            IStageDirector stageDirector,
            IWaveController waveController,
            IAudioController audioController,
            IBattleService battleService,
            ICameraCommand cameraCommand,
            ITowerPlacementCommand towerPlacementCommand,
            IBattleUserCommand battleUserCommand,
            IActorManager actorManager,
            ISubscriber<ActorStateMessage> actorTagSubscriber)
        {
            _stageDirector = stageDirector;
            _waveController = waveController;
            _battleService = battleService;
            _cameraCommand = cameraCommand;
            _towerPlacementCommand = towerPlacementCommand;
            _battleUserCommand = battleUserCommand;
            _actorManager = actorManager;
            audioController.PlayBGM(BGMAudioId.Stage);
            actorTagSubscriber.Subscribe(OnActorStateMessage).AddTo(ref DisposableBag);
        }

        protected override void OnStart()
        {
            RunAsync(new StageConfig
            {
                StageId = 1
            }).Forget();
        }

        private async UniTaskVoid RunAsync(StageConfig cfg)
        {
            _stageDirector.TrySetPhase(StagePhases.PreparingWave);
            _cameraCommand.Initialize(cfg.StageId);
            _waveController.Initialize(cfg.StageId);
            await _towerPlacementCommand.InitializeAsync(cfg.StageId); //data 만들기
            await _battleUserCommand.InitializeAsync(cfg.StageId);

            _stageDirector.TrySetMode(StageModes.Battle);
            _stageDirector.TrySetPhase(StagePhases.RunningWave);
        }

        public void Tick()
        {
            var dt = Time.deltaTime;
            _actorManager.Tick(dt);
            _waveController.Tick(dt);
        }

        private void OnActorStateMessage(ActorStateMessage message)
        {
            if (message.ActorId == ActorIds.Nexus &&
                message.Tag == StateTag.Die &&
                !_battleService.IsExistUnit(ActorIds.Nexus))
            {
                _stageDirector.TrySetPhase(StagePhases.Failed);
            }
        }
    }
}