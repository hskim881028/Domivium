using System;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Actors;
using Domivium.Client.Contents.Actors.Generated;
using Domivium.Client.Contents.Commands;
using Domivium.Client.Contents.Context;
using Domivium.Client.Contents.ReadModels;
using Domivium.Client.Contents.State;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Actors.Contract;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Director;
using Domivium.Client.Core.Factory;
using Domivium.Client.Core.Message;
using Domivium.Client.Core.State;
using MessagePipe;
using R3;
using UnityEngine;

namespace Domivium.Client.Contents.Services
{
    public sealed class BattleUserService : Disposable, IBattleUserReadModel, IBattleUserCommand
    {
        private readonly MasterDbService _masterDbService;
        private readonly CoordinateService _coordinateService;
        private readonly IStageDirector _director;
        private readonly IActorSpawner _actorSpawner;
        private readonly IBattleService _battleService;
        private readonly IActorFactory _actorFactory;

        private readonly ReactiveProperty<IBattleSystem> _selectedCharacter;
        private readonly ReactiveProperty<IBattleSystem> _pickedCharacter;
        private readonly ReactiveProperty<Vector3> _previewPosition;
        private readonly ReactiveProperty<Vector3> _targetPosition;

        public ReadOnlyReactiveProperty<IBattleSystem> SelectedCharacter => _selectedCharacter;
        public ReadOnlyReactiveProperty<IBattleSystem> PickedCharacter => _pickedCharacter;
        public ReadOnlyReactiveProperty<Vector3> PreviewPosition => _previewPosition;
        public ReadOnlyReactiveProperty<Vector3> TargetPosition => _targetPosition;

        public BattleUserService(
            MasterDbService masterDbService,
            CoordinateService coordinateService,
            IStageDirector director,
            IActorSpawner actorSpawner,
            IBattleService battleService,
            IActorFactory actorFactory,
            ISubscriber<ActorStateMessage> actorTagSubscriber,
            ISubscriber<SceneMessage> subscriber)
        {
            _selectedCharacter = new ReactiveProperty<IBattleSystem>().AddTo(ref DisposableBag);
            _pickedCharacter = new ReactiveProperty<IBattleSystem>().AddTo(ref DisposableBag);
            _previewPosition = new ReactiveProperty<Vector3>().AddTo(ref DisposableBag);
            _targetPosition = new ReactiveProperty<Vector3>().AddTo(ref DisposableBag);

            _masterDbService = masterDbService;
            _coordinateService = coordinateService;
            _director = director;
            _actorSpawner = actorSpawner;
            _battleService = battleService;
            _actorFactory = actorFactory;

            actorTagSubscriber.Subscribe(OnActorStateMessage).AddTo(ref DisposableBag);
            subscriber.Subscribe(OnSceneMessage).AddTo(ref DisposableBag);
        }

        public async UniTask InitializeAsync(int stageId)
        {
            await _actorSpawner.SpawnAsync(ActorIds.CharacterPathIndicator, ActorParam.Empty);
            await _actorSpawner.SpawnAsync(ActorIds.CharacterSelectIndicator, ActorParam.Empty);

            var stageRow = _masterDbService.DB.StageRowTable.FindByStageId(stageId);
            foreach (var row in stageRow)
            {
                if (row.CampType.FromCampTypeToActorId() != ActorIds.CharacterCamp) continue;

                var characterId = row.CampIndex switch
                {
                    0 => 1,
                    1 => 3,
                    2 => 2,
                    _ => throw new ArgumentOutOfRangeException()
                };

                if (characterId == 2) continue; // temp

                var character = _actorFactory.CreateCharacter(characterId, new Vector3Int(row.X, row.Y, 0));
                await _actorSpawner.SpawnAsync(ActorIds.Character, character);
            }
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
            return _director.TrySetMode(StageModes.MoveCharacter);
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
            _selectedCharacter.Value = _pickedCharacter.Value;
            _pickedCharacter.Value = null;
            return _director.TrySetMode(StageModes.Battle);
        }

        private void OnActorStateMessage(ActorStateMessage message)
        {
            if (_pickedCharacter.Value != null &&
                message.Id == _pickedCharacter.Value.Id &&
                message.Tag == StateTag.Die)
            {
                _pickedCharacter.Value = null;
            }

            if (_selectedCharacter.Value != null &&
                message.Id == _selectedCharacter.Value.Id &&
                message.Tag == StateTag.Die)
            {
                _selectedCharacter.Value = null;
            }
        }

        private void OnSceneMessage(SceneMessage message)
        {
            switch (message.Type)
            {
                case SceneMessageType.Unload:
                case SceneMessageType.Load:
                    _selectedCharacter.Value = null;
                    _pickedCharacter.Value = null;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}