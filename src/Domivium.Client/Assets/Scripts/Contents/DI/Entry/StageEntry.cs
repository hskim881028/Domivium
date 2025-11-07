using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Actors;
using Domivium.Client.Contents.Actors.Contract;
using Domivium.Client.Contents.Actors.Generated;
using Domivium.Client.Contents.Audio.Generated;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Audio;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Context;
using Domivium.Client.Core.Director;
using Domivium.Client.Core.Factory;
using Domivium.Client.Core.Input;
using Domivium.Client.Core.Provider;
using Domivium.Client.Core.Systems;
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
        private readonly IBattleAbilityFactory _abilityFactory;
        private readonly IActorParamFactory _actorParamFactory;
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
            IBattleAbilityFactory abilityFactory,
            IActorParamFactory actorParamFactory,
            IStageSystemCommand stageSystemCommand,
            ICharacterSystemCommand characterSystemCommand,
            ICameraSystemCommand cameraSystemCommand)
        {
            audioPlayer.PlayBGM(BGMAudioId.Stage);
            _stageDirector = stageDirector;
            _stageFieldProvider = stageFieldProvider;
            _actorManager = actorManager;
            _actorSpawner = actorSpawner;
            _abilityFactory = abilityFactory;
            _actorParamFactory = actorParamFactory;

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

            foreach (var cell in tilemap.cellBounds.allPositionsWithin)
            {
                if (tilemap.HasTile(cell)) continue;

                var position = new Vector2(cell.x + 0.427f, cell.y + 0.58f);
                await _actorSpawner.SpawnAsync(ActorIds.Prop, new PropParams(position));
            }

            var abilities = _abilityFactory.GetAbilities(ActorIds.Character);
            var actorParam = _actorParamFactory.CreateCharacter(1, Vector2.zero, abilities);
            var character = await _actorSpawner.SpawnAsync(ActorIds.Character, actorParam);
            if (character is CharacterPresenter characterPresenter)
            {
                _characterSystemCommand.Initialize(characterPresenter.BattleSystem);
                _cameraSystemCommand.Initialize(characterPresenter.Transform);
            }
            
            var monsterAbilities = _abilityFactory.GetAbilities(ActorIds.Monster);
            var monsterParam = _actorParamFactory.CreateMonster(1, new Vector2(2, -2), monsterAbilities);
            await _actorSpawner.SpawnAsync(ActorIds.Monster, monsterParam);

            _stageDirector.TrySetMode(StageMode.Run);
        }

        public void Tick()
        {
            _actorManager.Tick(Time.deltaTime);
        }
    }
}