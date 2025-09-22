using Domivium.Client.Contents.ReadModels;
using Domivium.Client.Contents.State;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Factory;
using R3;
using UnityEngine;

namespace Domivium.Client.Contents.Actors
{
    public class CharacterPresenter : UnitPresenter<Character>
    {
        public CharacterPresenter(
            Character actor,
            ISystemFactory systemFactory,
            IActorFinder actorFinder,
            IBattleReadModel read)
            : base(actor, systemFactory, actorFinder)
        {
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

        private void ForceMove(Vector3 position)
        {
            StateSystem.TryTransit(StateTags.Move);
            Actor.SetDestination(position);
        }
    }
}