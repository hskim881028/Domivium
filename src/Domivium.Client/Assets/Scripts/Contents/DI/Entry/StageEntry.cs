using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Actors;
using Domivium.Client.Contents.Audio.Generated;
using Domivium.Client.Contents.Commands;
using Domivium.Client.Contents.Context;
using Domivium.Client.Core.Audio;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Director;
using Domivium.Client.Core.Input;
using Domivium.Client.Core.Provider;
using Domivium.Client.Data.Config;
using UnityEngine;
using VContainer.Unity;

namespace Domivium.Client.Contents.DI.Entry
{
    public class StageEntry : Entry, ITickable
    {
        private readonly StageMapProvider _stageMapProvider;
        private readonly IStageDirector _stageDirector;
        private readonly ITowerPlacementCommand _towerPlacementCommand;
        private readonly IBattleCommand _battleCommand;
        private readonly ActorManager _actorManager;

        public StageEntry(
            IInputComposition inputComposition,
            IBattleCuePlayer cuePlayer,
            IStageDirector stageDirector,
            IAudioController audioController,
            ITowerPlacementCommand towerPlacementCommand,
            IBattleCommand battleCommand,
            ActorManager actorManager)
        {
            _stageDirector = stageDirector;
            _towerPlacementCommand = towerPlacementCommand;
            _battleCommand = battleCommand;
            _actorManager = actorManager;
            audioController.PlayBGM(BGMAudioId.Stage);
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
            await _towerPlacementCommand.InitializeAsync(cfg.StageId); //data 만들기
            await _battleCommand.InitializeAsync(cfg.StageId);

            _stageDirector.TrySetMode(StageModes.Battle);
            _stageDirector.TrySetPhase(StagePhases.RunningWave);
        }

        public void Tick()
        {
            var dt = Time.deltaTime;
            _actorManager.Tick(dt);
            _stageDirector.Tick(dt);
        }
    }
}