using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Commands;
using Domivium.Client.Contents.Context;
using Domivium.Client.Core.Director;
using Domivium.Client.Core.Input;
using Domivium.Client.Core.Provider;
using Domivium.Client.Data.Config;

namespace Domivium.Client.Contents.DI.Entry
{
    public class StageEntry : Entry
    {
        private readonly StageMapProvider _stageMapProvider;
        private readonly IStageDirector _stageDirector;
        private readonly ITowerPlacementCommand _towerPlacementCommand;
        private readonly IBattleCommand _battleCommand;

        public StageEntry(
            IInputComposition inputComposition,
            IStageDirector stageDirector,
            ITowerPlacementCommand towerPlacementCommand,
            IBattleCommand battleCommand)
        {
            _stageDirector = stageDirector;
            _towerPlacementCommand = towerPlacementCommand;
            _battleCommand = battleCommand;
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
    }
}