using System;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Actors;
using Domivium.Client.Contents.Actors.Contract;
using Domivium.Client.Contents.Actors.Generated;
using Domivium.Client.Contents.Audio.Generated;
using Domivium.Client.Contents.State;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Audio;
using Domivium.Client.Core.Factory;
using Domivium.Client.Core.Message;
using Domivium.Client.Core.Provider;
using Domivium.Client.Core.Systems;
using MessagePipe;
using R3;
using UnityEngine;

namespace Domivium.Client.Contents.Systems
{
    public sealed class StageSystem : Disposable, IStageSystem, IStageSystemCommand
    {
        private readonly StageFieldProvider _stageFieldProvider;
        private readonly IAudioPlayer _audioPlayer;
        private readonly IActorSpawner _actorSpawner;
        private readonly IActorManager _actorManager;
        private readonly IBattleAbilityFactory _abilityFactory;
        private readonly IActorParamFactory _actorParamFactory;
        private readonly IStageFieldSystemCommand _stageFieldSystemCommand;
        private readonly ICharacterSystemCommand _characterSystemCommand;
        private readonly ICameraSystemCommand _cameraSystemCommand;
        private readonly ReactiveProperty<StageMode> _mode = new();

        public ReadOnlyReactiveProperty<StageMode> Mode => _mode;

        public StageSystem(
            StageFieldProvider stageFieldProvider,
            IAudioPlayer audioPlayer,
            IActorSpawner actorSpawner,
            IActorManager actorManager,
            IBattleAbilityFactory abilityFactory,
            IActorParamFactory actorParamFactory,
            IStageFieldSystemCommand stageFieldSystemCommand,
            ICharacterSystemCommand characterSystemCommand,
            ICameraSystemCommand cameraSystemCommand,
            ISubscriber<SceneMessage> sceneSubscriber,
            ISubscriber<ActorStateMessage> actorStateSubscriber)
        {
            _stageFieldProvider = stageFieldProvider;
            _audioPlayer = audioPlayer;
            _actorSpawner = actorSpawner;
            _actorManager = actorManager;
            _abilityFactory = abilityFactory;
            _actorParamFactory = actorParamFactory;
            _stageFieldSystemCommand = stageFieldSystemCommand;
            _characterSystemCommand = characterSystemCommand;
            _cameraSystemCommand = cameraSystemCommand;
            sceneSubscriber.Subscribe(OnSceneMessage).AddTo(ref DisposableBag);
            actorStateSubscriber.Subscribe(OnActorStateMessage).AddTo(ref DisposableBag);
        }

        public async UniTaskVoid RunAsync(int stageId)
        {
            _audioPlayer.PlayBGM(BGMAudioId.Stage);
            var tilemap = _stageFieldProvider.Get(stageId);
            var stageField = await _actorSpawner.SpawnAsync(ActorIds.StageField, new StageFieldParams(tilemap));
            if (stageField is StageFieldPresenter stageFieldPresenter)
            {
                _stageFieldSystemCommand.InitializeAsync(stageFieldPresenter.Grid);
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

                var monsterAbilities = _abilityFactory.GetAbilities(ActorIds.Monster);
                var monsterParam = _actorParamFactory.CreateMonster(1, new Vector2(2, -2), monsterAbilities, characterPresenter.BattleSystem);
                await _actorSpawner.SpawnAsync(ActorIds.Monster, monsterParam);
            }

            _mode.Value = StageMode.Run;
        }

        private void OnSceneMessage(SceneMessage message)
        {
            switch (message.Type)
            {
                case SceneMessageType.Unload:
                case SceneMessageType.Load:
                    _mode.Value = StageMode.Prepare;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnActorStateMessage(ActorStateMessage message)
        {
            if (message.Tag == StateTags.Despawn && message.ActorId == ActorIds.Character)
            {
                _mode.Value = StageMode.Terminated;
            }
        }

        public void Tick(float deltaTime)
        {
            if (Mode.CurrentValue != StageMode.Run) return;

            _actorManager.Tick(deltaTime);
        }
    }
}