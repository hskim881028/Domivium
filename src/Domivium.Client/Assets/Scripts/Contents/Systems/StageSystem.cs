using System;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Actors;
using Domivium.Client.Contents.Audio.Generated;
using Domivium.Client.Contents.State;
using Domivium.Client.Contents.UI;
using Domivium.Client.Core;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Actors.Contract;
using Domivium.Client.Core.Audio;
using Domivium.Client.Core.Factory;
using Domivium.Client.Core.Message;
using Domivium.Client.Core.Systems;
using Domivium.Client.Core.UI.Navigation;
using Domivium.Client.Data.Item;
using MessagePipe;
using R3;
using UnityEngine;

namespace Domivium.Client.Contents.Systems
{
    public sealed class StageSystem : Disposable, IStageSystem, IStageSystemCommand
    {
        private readonly IAppContext _context;
        private readonly IAudioPlayer _audioPlayer;
        private readonly IUINavigation _uiNavigation;
        private readonly IActorSpawner _actorSpawner;
        private readonly IActorManager _actorManager;
        private readonly IBattleAbilityFactory _abilityFactory;
        private readonly IActorParamFactory _actorParamFactory;
        private readonly IStageFieldSystemCommand _stageFieldSystemCommand;
        private readonly IItemSystemCommand _itemSystemCommand;
        private readonly ICharacterSystemCommand _characterSystemCommand;
        private readonly ICameraSystemCommand _cameraSystemCommand;
        private readonly ILootSystemCommand _lootSystemCommand;

        public StageSystem(
            IAppContext context,
            IAudioPlayer audioPlayer,
            IUINavigation uiNavigation,
            IActorSpawner actorSpawner,
            IActorManager actorManager,
            IBattleAbilityFactory abilityFactory,
            IActorParamFactory actorParamFactory,
            IStageFieldSystemCommand stageFieldSystemCommand,
            IItemSystemCommand itemSystemCommand,
            ICharacterSystemCommand characterSystemCommand,
            ICameraSystemCommand cameraSystemCommand,
            ILootSystemCommand lootSystemCommand,
            ISubscriber<SceneMessage> sceneSubscriber,
            ISubscriber<ActorStateMessage> actorStateSubscriber)
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
            _itemSystemCommand = itemSystemCommand;
            _characterSystemCommand = characterSystemCommand;
            _cameraSystemCommand = cameraSystemCommand;
            sceneSubscriber.Subscribe(OnSceneMessage).AddTo(ref DisposableBag);
            actorStateSubscriber.Subscribe(OnActorStateMessage).AddTo(ref DisposableBag);
        }

        public async UniTaskVoid RunAsync(int stageId)
        {
            _audioPlayer.PlayBGM(BGMAudioId.Stage);
            var stageField = await _actorSpawner.SpawnAsync(ActorId.StageField, new ActorParam());
            if (stageField is not StageFieldPresenter stageFieldPresenter)
            {
                throw new InvalidOperationException();
            }
            _stageFieldSystemCommand.InitializeAsync(stageFieldPresenter.ColliderGrid);


            var abilities = _abilityFactory.GetAbilities(ActorId.Character);
            var actorParam = _actorParamFactory.CreateCharacter(1, new Vector2(7, 5), abilities);
            var character = await _actorSpawner.SpawnAsync(ActorId.Character, actorParam);
            if (character is not CharacterPresenter characterPresenter)
            {
                throw new InvalidOperationException();
            }

            _characterSystemCommand.Initialize(characterPresenter.BattleSystem);
            _cameraSystemCommand.Initialize(characterPresenter.Transform);
            _lootSystemCommand.Initialize(characterPresenter.Transform, stageFieldPresenter.StageProp);

            _itemSystemCommand.SetLootCapacity(8);
            _itemSystemCommand.SetInventoryCapacity(16);
            _itemSystemCommand.Add(ItemSlotType.Inventory, new ItemData(ItemType.Weapon, 1, 1));
            _itemSystemCommand.Add(ItemSlotType.Inventory, new ItemData(ItemType.Weapon, 2, 1));
            _itemSystemCommand.Add(ItemSlotType.Inventory, new ItemData(ItemType.Projectile, 1, 5));
            _itemSystemCommand.Add(ItemSlotType.Inventory, new ItemData(ItemType.Projectile, 2, 77));
            _itemSystemCommand.Add(ItemSlotType.Inventory, new ItemData(ItemType.Projectile, 1, 4));

            var monsterAbilities = _abilityFactory.GetAbilities(ActorId.Monster);
            var monsterParam = _actorParamFactory.CreateMonster(1, new Vector2(24, 9), monsterAbilities, characterPresenter.BattleSystem);
            await _actorSpawner.SpawnAsync(ActorId.Monster, monsterParam);

            var dummy = _actorParamFactory.CreateMonster(2, new Vector2(10, 6), monsterAbilities, characterPresenter.BattleSystem);
            await _actorSpawner.SpawnAsync(ActorId.Monster, dummy);

            _uiNavigation.ApplyUILayer(UILayers.Stage).Forget();
            _context.SetMode(StageMode.Run);
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

        private void OnActorStateMessage(ActorStateMessage message)
        {
            if (message.Tag == StateTags.Despawn && message.ActorId == ActorId.Character)
            {
                _context.SetMode(StageMode.Terminated);
            }
        }

        public void Tick(float deltaTime)
        {
            if (_context.Mode.CurrentValue != StageMode.Run) return;

            _actorManager.Tick(deltaTime);
            _lootSystemCommand.Tick();
        }
    }
}