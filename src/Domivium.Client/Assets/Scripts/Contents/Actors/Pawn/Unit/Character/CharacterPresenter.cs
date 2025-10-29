using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Actors.Generated;
using Domivium.Client.Contents.Battle;
using Domivium.Client.Contents.State;
using Domivium.Client.Contents.System.Model;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Actors.Contract;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Factory;
using Domivium.Client.Core.State;
using Domivium.Client.Data.Stat;
using R3;
using UnityEngine;

namespace Domivium.Client.Contents.Actors
{
    public class CharacterPresenter : UnitPresenter<Character>
    {
        public override ActorId ActorId => ActorIds.Character;

        private Vector2 _direction;
        private Vector2 _lookAt;
        private Vector2 _fire;
        private Vector3 _prePosition;

        public CharacterPresenter(
            Character actor,
            ISystemFactory systemFactory,
            IStageSystemModel stageSystemModel,
            ICharacterSystemModel inputSystemModel)
            : base(actor, systemFactory, stageSystemModel)
        {
            inputSystemModel.OnMove.Subscribe(OnMove).AddTo(ref DisposableBag);
            inputSystemModel.OnLookAt.Subscribe(OnLookAt).AddTo(ref DisposableBag);
            inputSystemModel.OnFire.Subscribe(OnFire).AddTo(ref DisposableBag);
            inputSystemModel.OnAvoid.Subscribe(OnAvoid).AddTo(ref DisposableBag);
        }

        public override UniTask ActivateAsync(CancellationToken token, ActorParam param)
        {
            base.ActivateAsync(token, param);
            _prePosition = BattleSystem.UnitPosition;
            return UniTask.CompletedTask;
        }

        protected override void OnMoveTick(float deltaTime)
        {
            base.OnMoveTick(deltaTime);

            _prePosition = BattleSystem.UnitPosition;
            var collider = BattleSystem.Collider;
            var speed = BattleSystem.Stat.RateValue(StatId.MoveSpeed);
            var delta = new Vector3(_direction.x, _direction.y, 0f);
            if (delta.sqrMagnitude > 1f)
            {
                delta.Normalize();
            }
            delta *= speed * deltaTime;
            var nextPosition = StageSystemModel.NextPosition(_prePosition, delta, collider);
            var context = BattleAbilityContext.Create(BattleAbilityIds.Move, BattleSystem, nextPosition);
            BattleSystem.TryActivateAbility(ref context);
        }

        private void OnMove(Vector2 value)
        {
            var tag = StateTag.Idle;
            if (value.sqrMagnitude > 0)
            {
                _direction = value;
                tag = StateTags.Move;
            }

            StateSystem.TryTransit(tag);
        }

        private void OnLookAt(Vector2 value)
        {
            _lookAt = value;
            Actor.SetAim(_lookAt);
        }

        private void OnFire(Vector2 value)
        {
            _fire = value;
            this.Log(_fire);
        }

        private void OnAvoid(float cooldown)
        {
            var position = BattleSystem.UnitPosition;
            var lastDirection = position - _prePosition;
            var collider = BattleSystem.Collider;
            var delta = new Vector3(lastDirection.x, lastDirection.y, 0f);
            delta.Normalize();

            var speed = BattleSystem.Stat.RateValue(StatId.MoveSpeed);
            delta *= speed;
            var nextPosition = StageSystemModel.NextPosition(position, delta, collider);
            var context = BattleAbilityContext.Create(BattleAbilityIds.Avoid, BattleSystem, nextPosition);
            BattleSystem.TryActivateAbility(ref context);
        }
    }
}