using Domivium.Client.Contents.Actors.Generated;
using Domivium.Client.Contents.State;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Factory;

namespace Domivium.Client.Contents.Actors
{
    public class MonsterPresenter : UnitPresenter<Monster>
    {
        public MonsterPresenter(
            Monster actor,
            ISystemFactory systemFactory,
            IActorFinder actorFinder)
            : base(actor, systemFactory, actorFinder) { }

        protected override void OnIdleTick()
        {
            if (ActorFinder.FindChaseTarget(TargetActionId, BattleSystem, out var nexus, out var nexusPosition))
            {
                Target = nexus;
                ChasePosition = nexusPosition;
            }
            else
            {
                if (!ActorFinder.FindChaseTarget(ActorIds.Tower, BattleSystem, out var tower, out var towerPosition))
                {
                    StateSystem.Terminate();
                    return;
                }

                Target = tower;
                ChasePosition = towerPosition;
            }

            StateSystem.TryTransit(StateTags.Chase);
            base.OnIdleTick();
        }
    }
}