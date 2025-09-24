using Domivium.Client.Contents.ReadModels;
using Domivium.Client.Contents.State;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Factory;
using R3;
using UnityEngine;

namespace Domivium.Client.Contents.Actors
{
    public class CharacterPresenter : UnitPresenter<Character>
    {
        private readonly IBattleReadModel _read;

        public CharacterPresenter(
            Character actor,
            ISystemFactory systemFactory,
            IActorFinder actorFinder,
            IBattleReadModel read)
            : base(actor, systemFactory, actorFinder)
        {
            _read = read;
            read.TargetPosition.Subscribe(ForceMove).AddTo(ref DisposableBag);
        }

        protected override void OnIdleTick()
        {
            if (StateSystem.Tag.CurrentValue == StateTags.Move) return;

            if (!ActorFinder.FindChaseTarget(TargetActionId, BattleSystem, out var target, out var chasePosition)) return;

            Target = target;
            ChasePosition = chasePosition;
            StateSystem.TryTransit(StateTags.Chase);
            base.OnIdleTick();
        }

        protected override void OnMoveTick()
        {
            if (Actor.IsRemainingDistance()) return;

            StateSystem.TryTransit(StateTags.Idle);

            base.OnMoveTick();
        }

        protected override void OnDamagedEffect(BattleEffectContext context)
        {
            CheckSwapTarget(context);
        }

        private void ForceMove(Vector3 position)
        {
            if (_read.PickedCharacter.CurrentValue.Id != Id) return;

            StateSystem.TryTransit(StateTags.Move);
            Actor.SetDestination(position);
        }
    }
}