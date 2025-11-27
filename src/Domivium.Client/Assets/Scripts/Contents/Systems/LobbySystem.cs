using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Actors;
using Domivium.Client.Contents.Audio.Generated;
using Domivium.Client.Contents.UI;
using Domivium.Client.Core;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Actors.Contract;
using Domivium.Client.Core.Audio;
using Domivium.Client.Core.Factory;
using Domivium.Client.Core.Message;
using Domivium.Client.Core.Systems;
using Domivium.Client.Core.UI.Navigation;
using MessagePipe;
using R3;
using UnityEngine;

namespace Domivium.Client.Contents.Systems
{
    public class LobbySystem : Disposable, ILobbySystem, ILobbySystemCommand
    {
        private readonly IAppContext _context;
        private readonly IAudioPlayer _audioPlayer;
        private readonly IUINavigation _uiNavigation;
        private readonly IActorSpawner _actorSpawner;
        private readonly IActorManager _actorManager;
        private readonly IBattleAbilityFactory _abilityFactory;
        private readonly IActorParamFactory _actorParamFactory;
        private readonly IStageFieldSystemCommand _stageFieldSystemCommand;
        private readonly ILootSystemCommand _lootSystemCommand;

        public LobbySystem(
            IAppContext context,
            IAudioPlayer audioPlayer,
            IUINavigation uiNavigation,
            IActorSpawner actorSpawner,
            IActorManager actorManager,
            IBattleAbilityFactory abilityFactory,
            IActorParamFactory actorParamFactory,
            IStageFieldSystemCommand stageFieldSystemCommand,
            ILootSystemCommand lootSystemCommand,
            ISubscriber<SceneMessage> sceneSubscriber)
        {
            _context = context;
            _audioPlayer = audioPlayer;
            _uiNavigation = uiNavigation;
            _actorSpawner = actorSpawner;
            _actorManager = actorManager;
            _lootSystemCommand = lootSystemCommand;
            _abilityFactory = abilityFactory;
            _actorParamFactory = actorParamFactory;
            _stageFieldSystemCommand = stageFieldSystemCommand;
            sceneSubscriber.Subscribe(OnSceneMessage).AddTo(ref DisposableBag);
        }

        public async UniTaskVoid RunAsync()
        {
            _audioPlayer.PlayBGM(BGMAudioId.Stage);
            var field = await _actorSpawner.SpawnAsync(ActorId.LobbyField, new ActorParam());
            if (field is not LobbyFieldPresenter lobbyFieldPresenter)
            {
                throw new InvalidOperationException();
            }

            _stageFieldSystemCommand.InitializeAsync(lobbyFieldPresenter.ColliderGrid);

            var abilities = _abilityFactory.GetAbilities(ActorId.Character);
            var actorParam = _actorParamFactory.CreateLobbyCharacter(1, new Vector2(3, 3), abilities);
            await _actorSpawner.SpawnAsync(ActorId.Character, actorParam);

            _lootSystemCommand.Initialize(new Dictionary<ushort, Vector2>());
            _uiNavigation.ApplyUILayer(UILayers.Lobby).Forget();
            _context.SetMode(StageMode.Run);
        }

        public void Tick(float deltaTime)
        {
            if (_context.Mode.CurrentValue != StageMode.Run) return;

            _actorManager.Tick(deltaTime);
            _lootSystemCommand.Tick(deltaTime);
        }

        private void OnSceneMessage(SceneMessage message)
        {
            switch (message.Type)
            {
                case SceneMessageType.Unload:
                case SceneMessageType.Load:
                    _context.SetMode(StageMode.Loading);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}