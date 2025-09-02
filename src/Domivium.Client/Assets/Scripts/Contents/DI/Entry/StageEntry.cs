using System.Linq;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Actors.Contract;
using Domivium.Client.Contents.Actors.Generated;
using Domivium.Client.Contents.Actors.StageMap;
using Domivium.Client.Contents.Commands;
using Domivium.Client.Contents.Context;
using Domivium.Client.Contents.Services;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Context;
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
        private readonly IActorSpawner _actorSpawner;
        private readonly ITowerPlacementCommand _towerPlacementCommand;

        public StageEntry(
            IInputComposition inputComposition,
            IStageDirector stageDirector,
            IActorSpawner actorSpawner,
            EnvironmentService environmentService,
            ITowerPlacementCommand towerPlacementCommand)
        {
            _stageDirector = stageDirector;
            _actorSpawner = actorSpawner;
            _towerPlacementCommand = towerPlacementCommand;
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
            var cells = _towerPlacementCommand.Initialize(cfg.StageId); //data 만들기
            var presenter = await _actorSpawner.SpawnAsync(ActorIds.Map, new StageMapParams(cells));
            if (presenter is StageMapPresenter stageMapPresenter)
            {
                _towerPlacementCommand.SetGrid(stageMapPresenter.Actor.Background);
            }
            
            await _actorSpawner.SpawnAsync(ActorIds.Character, new CharacterParams(cells.First()));

            _stageDirector.TrySetMode(StageModes.Battle);
            _stageDirector.TrySetPhase(StagePhases.PreparingWave);
        }
    }
}