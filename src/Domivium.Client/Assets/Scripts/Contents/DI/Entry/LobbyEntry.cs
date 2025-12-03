using System;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Actors;
using Domivium.Client.Contents.Audio.Generated;
using Domivium.Client.Contents.UI;
using Domivium.Client.Core;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Actors.Contract;
using Domivium.Client.Core.Audio;
using Domivium.Client.Core.Container;
using Domivium.Client.Core.Factory;
using Domivium.Client.Core.Scene;
using Domivium.Client.Core.Systems;
using Domivium.Client.Core.UI.Navigation;
using UnityEngine;
using VContainer.Unity;

namespace Domivium.Client.Contents.DI.Entry
{
    public class LobbyEntry : Entry, ITickable
    {
        private readonly IAppContext _context;
        private readonly IAudioPlayer _audioPlayer;
        private readonly IActorSpawner _actorSpawner;
        private readonly IActorManager _actorManager;
        private readonly IBattleAbilityFactory _abilityFactory;
        private readonly IActorParamFactory _actorParamFactory;
        private readonly IUserContainer _userContainer;
        private readonly IStageFieldSystemCommand _stageFieldSystemCommand;

        public LobbyEntry(
            IUINavigation uiNavigation,
            IAppContext context,
            IAudioPlayer audioPlayer,
            IActorSpawner actorSpawner,
            IActorManager actorManager,
            IBattleAbilityFactory abilityFactory,
            IActorParamFactory actorParamFactory,
            IUserContainer userContainer,
            IStageFieldSystemCommand stageFieldSystemCommand) : base(uiNavigation)
        {
            _context = context;
            _audioPlayer = audioPlayer;
            _actorSpawner = actorSpawner;
            _actorManager = actorManager;
            _abilityFactory = abilityFactory;
            _actorParamFactory = actorParamFactory;
            _userContainer = userContainer;
            _stageFieldSystemCommand = stageFieldSystemCommand;
        }

        protected override void OnStart()
        {
            RunAsync().Forget();
        }

        protected override void OnDispose()
        {
            _userContainer.SaveAsync().Forget();
            base.OnDispose();
        }

        public void Tick()
        {
            if (_context.Mode.CurrentValue != SceneMode.Run) return;

            _actorManager.Tick(Time.deltaTime);
        }

        private async UniTaskVoid RunAsync()
        {
            _context.SetMode(SceneMode.Loading);
            _audioPlayer.PlayBGM(BGMAudioId.Stage);
            await _userContainer.SaveAsync();

            var field = await _actorSpawner.SpawnAsync(ActorId.LobbyField, new ActorParam());
            if (field is not LobbyFieldPresenter lobbyFieldPresenter)
            {
                throw new InvalidOperationException();
            }

            _stageFieldSystemCommand.InitializeForLobbyAsync(lobbyFieldPresenter.ColliderGrid);

            var abilities = _abilityFactory.GetAbilities(ActorId.Character);
            var actorParam = _actorParamFactory.CreateLobbyCharacter(1, new Vector2(3, 3), abilities);
            await _actorSpawner.SpawnAsync(ActorId.Character, actorParam);

            UINavigation.ApplyUILayer(UILayers.Lobby).Forget();
            _context.SetMode(SceneMode.Run);
        }
    }
}