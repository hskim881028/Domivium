using Domivium.Client.Contents.Actors.Generated;
using Domivium.Client.Contents.ReadModels;
using Domivium.Client.Contents.State;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Factory;

namespace Domivium.Client.Contents.Actors
{
    public class MonsterPresenter : UnitPresenter<Monster>
    {
        public override ActorId ActorId => ActorIds.Monster;

        public MonsterPresenter(
            Monster actor,
            ISystemFactory systemFactory,
            IBattleService battleService)
            : base(actor, systemFactory, battleService) { }

        protected override void OnIdleTick()
        {
            if (BattleService.FindChaseTarget(TargetActionId, BattleSystem, out var nexus, out var nexusPosition))
            {
                Target = nexus;
                ChasePosition = nexusPosition;
            }
            else
            {
                if (!BattleService.FindChaseTarget(ActorIds.Tower, BattleSystem, out var tower, out var towerPosition))
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

        protected override bool CheckForceSwapTarget() => Target.ActorId == ActorIds.Nexus;

        protected override void OnDamagedEffect(BattleEffectContext context)
        {
            CheckSwapTarget(context);
        }
    }
}