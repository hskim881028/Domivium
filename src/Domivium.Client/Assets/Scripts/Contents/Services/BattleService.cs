using System;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Actors;
using Domivium.Client.Contents.Actors.Contract;
using Domivium.Client.Contents.Actors.Generated;
using Domivium.Client.Contents.Commands;
using Domivium.Client.Contents.ReadModels;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Actors.Contract;
using Domivium.Client.Core.Director;
using Domivium.Client.Core.Message;
using Domivium.Client.Core.Utility;
using MessagePipe;
using R3;
using UnityEngine;
using DisposableBag = R3.DisposableBag;

namespace Domivium.Client.Contents.Services
{
    public sealed class BattleService : IBattleReadModel, IBattleCommand, IDisposable
    {
        private readonly IStageDirector _director;
        private readonly IActorSpawner _actorSpawner;
        private readonly ICameraReadModel _cameraRead;

        private DisposableBag _disposable;
        private bool _isDisposed;

        private readonly ReactiveProperty<Transform> _pickedCharacter = new();
        private readonly ReactiveProperty<Vector3> _previewPosition = new();
        private readonly ReactiveProperty<Vector3> _targetPosition = new();

        public ReadOnlyReactiveProperty<Transform> PickedCharacter => _pickedCharacter;
        public ReadOnlyReactiveProperty<Vector3> PreviewPosition => _previewPosition;
        public ReadOnlyReactiveProperty<Vector3> TargetPosition => _targetPosition;

        public BattleService(
            IStageDirector director,
            IActorSpawner actorSpawner,
            ICameraReadModel cameraRead,
            MasterDbService masterDbService,
            ISubscriber<SceneMessage> subscriber)
        {
            _director = director;
            _actorSpawner = actorSpawner;
            _cameraRead = cameraRead;
            subscriber.Subscribe(OnSceneMessage).AddTo(ref _disposable);

            var characterRow = masterDbService.DB.CharacterRowTable.FindById(1);
            this.Log($"{characterRow.Id} / {characterRow.Attack} / {characterRow.AttackRange} /" +
                     $" {characterRow.Health} / {characterRow.Defense} / {characterRow.Job} / {characterRow.Speed}");

            var monster = masterDbService.DB.MonsterRowTable.FindById(1);
            this.Log($"{monster.Id} / {monster.Attack} / {monster.AttackRange} /" +
                     $" {monster.Health} / {monster.Defense} / {monster.Job} / {monster.Speed}");

            var tw = masterDbService.DB.TowerRowTable.FindById(1);
            this.Log($"{tw.Id} / {tw.Attack} / {tw.AttackRange} /" +
                     $" {tw.Health} / {tw.Defense} / {tw.Job}");
        }

        public async UniTask InitializeAsync(int stageId)
        {
            await _actorSpawner.SpawnAsync(ActorIds.Character, new CharacterParams());
            await _actorSpawner.SpawnAsync(ActorIds.CharacterPathIndicator, ActorParam.Empty);
        }

        public bool PickCharacter(Vector2 position)
        {
            if (_pickedCharacter.Value != null) return false;

            if (!CoordinateUtils.TryScreenToCollider(_cameraRead.MainCamera, position, out var collider)) return false;

            var character = collider.GetComponent<Character>();
            if (character == null) return false;

            _pickedCharacter.Value = character.transform;
            _previewPosition.Value = character.transform.position;
            return true;
        }

        public bool UpdateMoveTarget(Vector2 position)
        {
            if (!CoordinateUtils.TryScreenToWorld(_cameraRead.MainCamera, position, out var worldPosition)) return true;

            _previewPosition.Value = worldPosition;
            return true;
        }

        public bool SelectCharacter(Vector2 position)
        {
            if (_pickedCharacter.Value != null)
            {
                var dist = Vector3.Distance(_pickedCharacter.Value.position, _previewPosition.Value);
                if (dist > 0.2f)
                {
                    _targetPosition.Value = _previewPosition.Value;
                }
            }

            _previewPosition.Value = Vector3.zero;
            _pickedCharacter.Value = null;
            return true;
        }

        public void Dispose()
        {
            if (_isDisposed) return;

            _isDisposed = true;
            _disposable.Dispose();
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