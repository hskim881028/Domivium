using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Actors;
using Domivium.Client.Contents.Actors.Contract;
using Domivium.Client.Contents.Actors.Generated;
using Domivium.Client.Contents.Audio.Generated;
using Domivium.Client.Contents.System.Command;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Audio;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Context;
using Domivium.Client.Core.Director;
using Domivium.Client.Core.Factory;
using Domivium.Client.Core.Input;
using Domivium.Client.Core.Provider;
using Domivium.Client.Data.Config;
using UnityEngine;
using VContainer.Unity;

namespace Domivium.Client.Contents.DI.Entry
{
    public class StageEntry : Entry, ITickable
    {
        private readonly IStageDirector _stageDirector;
        private readonly IActorManager _actorManager;
        private readonly IActorSpawner _actorSpawner;
        private readonly IActorFactory _actorFactory;
        private readonly IStageSystemCommand _stageSystemCommand;
        private readonly ICharacterSystemCommand _characterSystemCommand;
        private readonly ICameraSystemCommand _cameraSystemCommand;
        private readonly StageFieldProvider _stageFieldProvider;

        public StageEntry(
            IInputComposition inputComposition,
            IBattleEffectPool effectPool,
            IBattleCuePlayer cuePlayer,
            IAudioPlayer audioPlayer,
            IStageDirector stageDirector,
            StageFieldProvider stageFieldProvider,
            IActorManager actorManager,
            IActorSpawner actorSpawner,
            IActorFactory actorFactory,
            IStageSystemCommand stageSystemCommand,
            ICharacterSystemCommand characterSystemCommand,
            ICameraSystemCommand cameraSystemCommand)
        {
            audioPlayer.PlayBGM(BGMAudioId.Stage);
            _stageDirector = stageDirector;
            _stageFieldProvider = stageFieldProvider;
            _actorManager = actorManager;
            _actorSpawner = actorSpawner;
            _actorFactory = actorFactory;

            _stageSystemCommand = stageSystemCommand;
            _characterSystemCommand = characterSystemCommand;
            _cameraSystemCommand = cameraSystemCommand;
        }

        protected override void OnStart()
        {
            RunAsync(new StageConfig
            {
                StageId = 1,
                StartSoul = 100,
                RerollCost = 2,
                TowerLimit = 3,
            }).Forget();
        }

        private async UniTaskVoid RunAsync(StageConfig cfg)
        {
            var tilemap = _stageFieldProvider.Get(cfg.StageId);
            var stageField = await _actorSpawner.SpawnAsync(ActorIds.StageField, new StageFieldParams(tilemap));
            if (stageField is StageFieldPresenter stageFieldPresenter)
            {
                _stageSystemCommand.InitializeAsync(stageFieldPresenter.Grid);
            }

            var actorParam = _actorFactory.CreateCharacter(1, new Vector3Int(0, 0, 0));
            var character = await _actorSpawner.SpawnAsync(ActorIds.Character, actorParam);
            if (character is CharacterPresenter characterPresenter)
            {
                _characterSystemCommand.Initialize(characterPresenter.BattleSystem);
                _cameraSystemCommand.Initialize(characterPresenter.BattleSystem.Unit);
            }

            _stageDirector.TrySetMode(StageMode.Run);
        }

        public void Tick()
        {
            _actorManager.Tick(Time.deltaTime);
        }
    }
}