using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Audio.Generated;
using Domivium.Client.Contents.Commands;
using Domivium.Client.Contents.Context;
using Domivium.Client.Contents.Services;
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
        private readonly UnitService _unitService;

        public StageEntry(
            IInputComposition inputComposition,
            IBattleCuePlayer cuePlayer,
            IStageDirector stageDirector,
            IAudioController audioController,
            ITowerPlacementCommand towerPlacementCommand,
            IBattleCommand battleCommand,
            UnitService unitService)
        {
            _stageDirector = stageDirector;
            _towerPlacementCommand = towerPlacementCommand;
            _battleCommand = battleCommand;
            _unitService = unitService;
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
            await _towerPlacementCommand.InitializeAsync(cfg.StageId); //data 만들기
            await _battleCommand.InitializeAsync(cfg.StageId);

            _stageDirector.TrySetMode(StageModes.Battle);
            _stageDirector.TrySetPhase(StagePhases.PreparingWave);
        }

        public void Tick()
        {
            _unitService.Tick(Time.deltaTime);
        }
    }
}