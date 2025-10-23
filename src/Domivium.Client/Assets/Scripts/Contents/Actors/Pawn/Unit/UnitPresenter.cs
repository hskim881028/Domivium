using Domivium.Client.Contents.Battle;
using Domivium.Client.Contents.ReadModels;
using Domivium.Client.Contents.State;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Factory;
using Domivium.Client.Data.Stat;
using UnityEngine;

namespace Domivium.Client.Contents.Actors
{
    public abstract class UnitPresenter<TUnit> : PawnPresenter<TUnit>, IUnitPresenter where TUnit : Unit
    {
        private const float UpdateDistance = 0.5f;

        protected UnitPresenter(TUnit actor, ISystemFactory systemFactory, IBattleService battleService)
            : base(actor, systemFactory, battleService) { }

        protected override bool OnChaseTick()
        {
            if (!base.OnChaseTick()) return false;
            
            if (IsEmptyTarget())
            {
                StateSystem.TryTransit(StateTags.Idle);
                return false;
            }

            if (BattleCalculator.CanBattle(BattleSystem, Target))
            {
                StateSystem.TryTransit(StateTags.Battle);
                return false;
            }

            if (Vector3.Distance(BattleSystem.UnitPosition, Target.UnitPosition) < UpdateDistance) return false;

            if (BattleService.RecalculateChasePosition(BattleSystem, Target, out var chasePosition))
            {
                ChasePosition = chasePosition;
                Actor.SetDestination(ChasePosition);
            }
            else
            {
                StateSystem.TryTransit(StateTags.Idle);
            }

            return true;
        }

        protected override void OnChase()
        {
            Actor.SetDestination(ChasePosition);
            base.OnChase();
        }

        protected override void OnMoveSpeedStatChanged()
        {
            var speed = BattleSystem.Stat.RateValue(StatId.MoveSpeed);
            Actor.SetSpeed(speed);
            base.OnMoveSpeedStatChanged();
        }
    }
}