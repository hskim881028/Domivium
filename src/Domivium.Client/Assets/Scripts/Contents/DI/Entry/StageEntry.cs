using System;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Actors;
using Domivium.Client.Contents.Audio.Generated;
using Domivium.Client.Contents.State;
using Domivium.Client.Contents.UI;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Actors.Contract;
using Domivium.Client.Core.Audio;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Container;
using Domivium.Client.Core.Context;
using Domivium.Client.Core.Factory;
using Domivium.Client.Core.Message;
using Domivium.Client.Core.Scene;
using Domivium.Client.Core.Systems;
using Domivium.Client.Core.UI.Navigation;
using Domivium.Client.Data.Config;
using MessagePipe;
using R3;
using UnityEngine;
using VContainer.Unity;

namespace Domivium.Client.Contents.DI.Entry
{
    public class StageEntry : Entry, ITickable
    {
        private readonly IAppContext _context;
        private readonly IAudioPlayer _audioPlayer;
        private readonly IActorSpawner _actorSpawner;
        private readonly IActorManager _actorManager;
        private readonly IBattleAbilityFactory _abilityFactory;
        private readonly IActorParamFactory _actorParamFactory;
        private readonly IStageFieldSystemCommand _stageFieldSystemCommand;
        private readonly IUserContainer _userContainer;

        public StageEntry(
            IUINavigation uiNavigation,
            IBattleEffectPool effectPool,
            IBattleCuePlayer cuePlayer,
            IAppContext context,
            IAudioPlayer audioPlayer,
            IActorSpawner actorSpawner,
            IActorManager actorManager,
            IBattleAbilityFactory abilityFactory,
            IActorParamFactory actorParamFactory,
            IStageFieldSystemCommand stageFieldSystemCommand,
            IUserContainer userContainer,
            ISubscriber<ActorStateMessage> actorStateSubscriber) : base(uiNavigation)
        {
            _context = context;
            _audioPlayer = audioPlayer;
            _actorSpawner = actorSpawner;
            _actorManager = actorManager;
            _userContainer = userContainer;
            _abilityFactory = abilityFactory;
            _actorParamFactory = actorParamFactory;
            _stageFieldSystemCommand = stageFieldSystemCommand;
            actorStateSubscriber.Subscribe(OnActorStateMessage).AddTo(ref DisposableBag);
        }

        protected override void OnStart()
        {
            var config = new StageConfig { StageId = 1 };
            RunAsync(config.StageId).Forget();
        }

        public void Tick()
        {
            if (_context.Mode.CurrentValue != SceneMode.Run) return;

            _actorManager.Tick(Time.deltaTime);
            _userContainer.Tick(Time.deltaTime);
        }

        private async UniTaskVoid RunAsync(int stageId)
        {
            _context.SetMode(SceneMode.Loading);
            _audioPlayer.PlayBGM(BGMAudioId.Stage);
            var stageField = await _actorSpawner.SpawnAsync(ActorId.StageField, new ActorParam());
            if (stageField is not StageFieldPresenter stageFieldPresenter)
            {
                throw new InvalidOperationException();
            }
            _stageFieldSystemCommand.InitializeForStageAsync(stageFieldPresenter.ColliderGrid, stageFieldPresenter.Loots);

            var abilities = _abilityFactory.GetAbilities(ActorId.Character);
            var actorParam = _actorParamFactory.CreateCharacter(1, new Vector2(7, 5), abilities);
            var character = await _actorSpawner.SpawnAsync(ActorId.Character, actorParam);
            if (character is not CharacterPresenter characterPresenter)
            {
                throw new InvalidOperationException();
            }

            await _userContainer.InitializeLootAsync(characterPresenter.Transform, stageId);

            var monsterAbilities = _abilityFactory.GetAbilities(ActorId.Monster);
            var dummy = _actorParamFactory.CreateMonster(1, new Vector2(10, 6), monsterAbilities, characterPresenter.BattleSystem);
            await _actorSpawner.SpawnAsync(ActorId.Monster, dummy);

            var monsterParam = _actorParamFactory.CreateMonster(2, new Vector2(24, 9), monsterAbilities, characterPresenter.BattleSystem);
            await _actorSpawner.SpawnAsync(ActorId.Monster, monsterParam);

            UINavigation.ApplyUILayer(UILayers.Stage).Forget();
            _context.SetMode(SceneMode.Run);
        }

        private void OnActorStateMessage(ActorStateMessage message)
        {
            if (message.Tag == StateTags.Despawn && message.ActorId == ActorId.Character)
            {
                _context.SetMode(SceneMode.Terminated);
            }
        }
    }
}