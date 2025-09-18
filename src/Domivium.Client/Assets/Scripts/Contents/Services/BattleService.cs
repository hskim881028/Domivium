using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Actors;
using Domivium.Client.Contents.Actors.Contract;
using Domivium.Client.Contents.Actors.Generated;
using Domivium.Client.Contents.Battle;
using Domivium.Client.Contents.Commands;
using Domivium.Client.Contents.ReadModels;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Actors.Contract;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Director;
using Domivium.Client.Core.Message;
using Domivium.Client.Core.Utility;
using MessagePipe;
using R3;
using UnityEngine;

namespace Domivium.Client.Contents.Services
{
    public sealed class BattleService : Disposable, IBattleReadModel, IBattleCommand
    {
        private readonly MasterDbService _masterDbService;
        private readonly IStageDirector _director;
        private readonly IActorSpawner _actorSpawner;
        private readonly IBattleAbilityFactory _abilityFactory;
        private readonly ICameraReadModel _cameraRead;

        private readonly ReactiveProperty<Transform> _pickedCharacter = new();
        private readonly ReactiveProperty<Vector3> _previewPosition = new();
        private readonly ReactiveProperty<Vector3> _targetPosition = new();

        public ReadOnlyReactiveProperty<Transform> PickedCharacter => _pickedCharacter;
        public ReadOnlyReactiveProperty<Vector3> PreviewPosition => _previewPosition;
        public ReadOnlyReactiveProperty<Vector3> TargetPosition => _targetPosition;

        public BattleService(
            MasterDbService masterDbService,
            IStageDirector director,
            IActorSpawner actorSpawner,
            IBattleAbilityFactory abilityFactory,
            ICameraReadModel cameraRead,
            ISubscriber<SceneMessage> subscriber)
        {
            _masterDbService = masterDbService;
            _director = director;
            _actorSpawner = actorSpawner;
            _abilityFactory = abilityFactory;
            _cameraRead = cameraRead;
            subscriber.Subscribe(OnSceneMessage).AddTo(ref DisposableBag);
        }

        public async UniTask InitializeAsync(int stageId)
        {
            var characterRow = _masterDbService.DB.CharacterRowTable.FindById(1);
            var characterAbilities = new List<BattleAbilitySpec> { _abilityFactory.Create(BattleAbilityIds.Slash) };
            var characterContext = new UnitContext(characterRow);
            await _actorSpawner.SpawnAsync(ActorIds.Character, new UnitParams(characterContext, characterAbilities));
            await _actorSpawner.SpawnAsync(ActorIds.CharacterPathIndicator, ActorParam.Empty);

            var monsterRow = _masterDbService.DB.MonsterRowTable.FindById(1);
            var monsterAbilities = new List<BattleAbilitySpec> { _abilityFactory.Create(BattleAbilityIds.Slash) };
            var monsterContext = new UnitContext(monsterRow);
            await _actorSpawner.SpawnAsync(ActorIds.Monster, new UnitParams(monsterContext, monsterAbilities));
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