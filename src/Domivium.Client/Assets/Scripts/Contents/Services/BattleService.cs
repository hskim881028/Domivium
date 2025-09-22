using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Actors;
using Domivium.Client.Contents.Actors.Contract;
using Domivium.Client.Contents.Actors.Generated;
using Domivium.Client.Contents.Battle;
using Domivium.Client.Contents.Commands;
using Domivium.Client.Contents.ReadModels;
using Domivium.Client.Contents.State;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Actors.Contract;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Message;
using MessagePipe;
using R3;
using UnityEngine;

namespace Domivium.Client.Contents.Services
{
    public sealed class BattleService : Disposable, IBattleReadModel, IBattleCommand
    {
        private readonly MasterDbService _masterDbService;
        private readonly CoordinateService _coordinateService;
        private readonly IActorSpawner _actorSpawner;
        private readonly IActorFinder _actorFinder;
        private readonly IBattleAbilityFactory _abilityFactory;

        private readonly ReactiveProperty<IBattleSystem> _pickedCharacter;
        private readonly ReactiveProperty<Vector3> _previewPosition;
        private readonly ReactiveProperty<Vector3> _targetPosition;

        public ReadOnlyReactiveProperty<IBattleSystem> PickedCharacter => _pickedCharacter;
        public ReadOnlyReactiveProperty<Vector3> PreviewPosition => _previewPosition;
        public ReadOnlyReactiveProperty<Vector3> TargetPosition => _targetPosition;

        public BattleService(
            MasterDbService masterDbService,
            CoordinateService coordinateService,
            IActorSpawner actorSpawner,
            IActorFinder actorFinder,
            IBattleAbilityFactory abilityFactory,
            ISubscriber<SceneMessage> subscriber)
        {
            _pickedCharacter = new ReactiveProperty<IBattleSystem>().AddTo(ref DisposableBag);
            _previewPosition = new ReactiveProperty<Vector3>().AddTo(ref DisposableBag);
            _targetPosition = new ReactiveProperty<Vector3>().AddTo(ref DisposableBag);

            _masterDbService = masterDbService;
            _coordinateService = coordinateService;
            _actorSpawner = actorSpawner;
            _actorFinder = actorFinder;
            _abilityFactory = abilityFactory;

            subscriber.Subscribe(OnSceneMessage).AddTo(ref DisposableBag);
        }

        public async UniTask InitializeAsync(int stageId)
        {
            var nexusRow = _masterDbService.DB.NexusRowTable.FindById(1);
            var ability = _abilityFactory.Create(BattleAbilityIds.Slash);
            var abilities = new List<BattleAbilitySpec> { ability };
            var nexusContext = new UnitContext(nexusRow);
            await _actorSpawner.SpawnAsync(ActorIds.Nexus, new UnitParams(Vector3Int.zero, nexusContext, abilities));

            var characterRow = _masterDbService.DB.CharacterRowTable.FindById(1);
            var characterAbilities = new List<BattleAbilitySpec> { _abilityFactory.Create(BattleAbilityIds.Slash) };
            var characterContext = new UnitContext(characterRow);
            var characterPosition = _coordinateService.GetPosition(new Vector3(2, 0, 2));
            await _actorSpawner.SpawnAsync(ActorIds.Character, new UnitParams(characterPosition, characterContext, characterAbilities));
            await _actorSpawner.SpawnAsync(ActorIds.CharacterPathIndicator, ActorParam.Empty);

            var monsterRow = _masterDbService.DB.MonsterRowTable.FindById(1);
            var monsterAbilities = new List<BattleAbilitySpec> { _abilityFactory.Create(BattleAbilityIds.Slash) };
            var monsterContext = new UnitContext(monsterRow);
            var monsterPosition = _coordinateService.GetPosition(new Vector3(-2, 0, -2));
            await _actorSpawner.SpawnAsync(ActorIds.Monster, new UnitParams(monsterPosition, monsterContext, monsterAbilities));
        }

        public bool PickCharacter(Vector2 position)
        {
            if (_pickedCharacter.Value != null) return false;

            if (!_coordinateService.TryScreenToCollider(position, out var collider)) return false;

            var character = collider.GetComponent<Character>();
            if (character == null) return false;

            if (!_actorFinder.FindTarget(ActorIds.Character, character.Id, out var target)) return false;

            if (target.State == StateTags.Die || target.State == StateTags.Despawn) return false;

            _pickedCharacter.Value = target;
            _previewPosition.Value = target.UnitPosition;
            return true;
        }

        public bool UpdateMoveTarget(Vector2 position)
        {
            if (_pickedCharacter.Value == null ||
                _pickedCharacter.Value.State == StateTags.Die ||
                _pickedCharacter.Value.State == StateTags.Despawn)
            {
                _previewPosition.Value = Vector3.zero;
                _pickedCharacter.Value = null;
                return false;
            }

            if (!_coordinateService.TryScreenToWorld(position, out var worldPosition)) return true;

            _previewPosition.Value = worldPosition;
            return true;
        }

        public bool SelectCharacter(Vector2 position)
        {
            if (_pickedCharacter.Value != null &&
                _pickedCharacter.Value.State != StateTags.Die &&
                _pickedCharacter.Value.State != StateTags.Despawn)
            {
                var dist = Vector3.Distance(_pickedCharacter.Value.UnitPosition, _previewPosition.Value);
                if (dist > 0.2f)
                {
                    _targetPosition.Value = _previewPosition.Value;
                }
            }

            _previewPosition.Value = Vector3.zero;
            _pickedCharacter.Value = null;
            return true;
        }

        private void OnSceneMessage(SceneMessage message)
        {
            switch (message.Type)
            {
                case SceneMessageType.Unload:
                case SceneMessageType.Load:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}