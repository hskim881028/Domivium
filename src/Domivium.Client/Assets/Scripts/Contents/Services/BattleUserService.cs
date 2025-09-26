using System;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Actors;
using Domivium.Client.Contents.Actors.Generated;
using Domivium.Client.Contents.Commands;
using Domivium.Client.Contents.ReadModels;
using Domivium.Client.Contents.State;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Actors.Contract;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Factory;
using Domivium.Client.Core.Message;
using MessagePipe;
using R3;
using UnityEngine;

namespace Domivium.Client.Contents.Services
{
    public sealed class BattleUserService : Disposable, IBattleUserReadModel, IBattleUserCommand
    {
        private readonly CoordinateService _coordinateService;
        private readonly IActorSpawner _actorSpawner;
        private readonly IBattleService _battleService;
        private readonly IActorFactory _actorFactory;

        private readonly ReactiveProperty<IBattleSystem> _pickedCharacter;
        private readonly ReactiveProperty<Vector3> _previewPosition;
        private readonly ReactiveProperty<Vector3> _targetPosition;

        public ReadOnlyReactiveProperty<IBattleSystem> PickedCharacter => _pickedCharacter;
        public ReadOnlyReactiveProperty<Vector3> PreviewPosition => _previewPosition;
        public ReadOnlyReactiveProperty<Vector3> TargetPosition => _targetPosition;

        public BattleUserService(
            CoordinateService coordinateService,
            IActorSpawner actorSpawner,
            IBattleService battleService,
            IActorFactory actorFactory,
            ISubscriber<SceneMessage> subscriber)
        {
            _pickedCharacter = new ReactiveProperty<IBattleSystem>().AddTo(ref DisposableBag);
            _previewPosition = new ReactiveProperty<Vector3>().AddTo(ref DisposableBag);
            _targetPosition = new ReactiveProperty<Vector3>().AddTo(ref DisposableBag);

            _coordinateService = coordinateService;
            _actorSpawner = actorSpawner;
            _battleService = battleService;
            _actorFactory = actorFactory;

            subscriber.Subscribe(OnSceneMessage).AddTo(ref DisposableBag);
        }

        public async UniTask InitializeAsync(int stageId)
        {
            await _actorSpawner.SpawnAsync(ActorIds.CharacterPathIndicator, ActorParam.Empty);

            var character = _actorFactory.CreateCharacter(1, new Vector3(2, 0, 2));
            await _actorSpawner.SpawnAsync(ActorIds.Character, character);

            var support = _actorFactory.CreateCharacter(3, new Vector3(-2, 0, 2));
            await _actorSpawner.SpawnAsync(ActorIds.Character, support);
        }

        public bool PickCharacter(Vector2 position)
        {
            if (_pickedCharacter.Value != null) return false;

            if (!_coordinateService.TryScreenToCollider(position, out var collider)) return false;

            var character = collider.GetComponent<Character>();
            if (character == null) return false;

            if (!_battleService.FindTarget(ActorIds.Character, character.Id, out var target)) return false;

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