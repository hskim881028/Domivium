using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Actors.Contract;
using Domivium.Client.Contents.Actors.Generated;
using Domivium.Client.Contents.Battle;
using Domivium.Client.Contents.State;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Actors.Contract;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Factory;
using Domivium.Client.Data.Stat;
using UnityEngine;

namespace Domivium.Client.Contents.Actors
{
    public abstract class UnitPresenter<TUnit> : ActorPresenter<TUnit>, IUnitPresenter where TUnit : Unit
    {
        private const float UpdateDistance = 0.5f;

        protected readonly IActorFinder ActorFinder;
        protected IBattleSystem Target;
        protected Vector3 ChasePosition;
        protected ActorId TargetActionId;

        public IBattleSystem BattleSystem { get; }

        protected UnitPresenter(
            TUnit actor,
            ISystemFactory systemFactory,
            IActorFinder actorFinder)
            : base(actor, systemFactory)
        {
            BattleSystem = systemFactory.CreateBattle(actor.transform, StateSystem.Tag);
            ActorFinder = actorFinder;
        }

        public override UniTask ActivateAsync(CancellationToken token, ActorParam param)
        {
            var p = param.As<UnitParams>();
            var row = p.UnitContext;

            BattleSystem.Reset();
            BattleSystem.SetType(row.Job.ToUnitType());
            TargetActionId = row.Target.ToActorId();

            BattleSystem.Stat.Register(StatId.Health, row.Health, OnHealthStatChanged);
            BattleSystem.Stat.Register(StatId.Attack, row.Attack, OnAttackStatChanged);
            BattleSystem.Stat.Register(StatId.Defense, row.Defense, OnDefenseStatChanged);

            BattleSystem.Stat.Register(StatId.MoveSpeed, row.MoveSpeed, OnMoveSpeedStatChanged);
            BattleSystem.Stat.Register(StatId.AttackSpeed, row.AttackSpeed, OnAttackSpeedStatChanged);

            BattleSystem.Stat.Register(StatId.HitRange, row.HitRange, OnHitRangeStatChanged);
            BattleSystem.Stat.Register(StatId.AttackRange, row.AttackRange, OnAttackRangeStatChanged);
            BattleSystem.Stat.Register(StatId.DetectionRange, row.DetectionRange, OnDetectionRangeStatChanged);

            BattleSystem.Stat.Register(StatId.CriticalRate, row.CriticalRate, OnCriticalRateStatChanged);
            BattleSystem.Stat.Register(StatId.CriticalDamage, row.CriticalDamage, OnCriticalDamageStatChanged);

            BattleSystem.Gauge.Register(StatId.Health, row.Health, OnHealthGaugeChanged);

            foreach (var ability in p.Abilities)
            {
                BattleSystem.GrantAbility(ability);
            }

            StateSystem.TryTransit(StateTags.Idle);
            return base.ActivateAsync(token, param);
        }

        public override void Deactivate()
        {
            BattleSystem.Reset();
            base.Deactivate();
        }

        public override void Tick(float deltaTime)
        {
            BattleSystem.Tick(deltaTime);
            base.Tick(deltaTime);
        }

        protected bool IsEmptyTarget() => Target == null || Target.State == StateTags.Die || Target.State == StateTags.Despawn;

        protected override void OnChaseTick()
        {
            if (IsEmptyTarget())
            {
                StateSystem.TryTransit(StateTags.Idle);
                return;
            }

            if (BattleCalculator.CanBattle(BattleSystem, Target))
            {
                StateSystem.TryTransit(StateTags.Battle);
                return;
            }

            if (Vector3.Distance(BattleSystem.UnitPosition, Target.UnitPosition) < UpdateDistance) return;

            if (ActorFinder.RecalculateChasePosition(BattleSystem, Target, out var chasePosition))
            {
                ChasePosition = chasePosition;
                Actor.SetDestination(ChasePosition);
            }
            else
            {
                StateSystem.TryTransit(StateTags.Idle);
            }

            base.OnChaseTick();
        }

        protected override void OnBattleTick()
        {
            if (IsEmptyTarget())
            {
                StateSystem.TryTransit(StateTags.Idle);
                return;
            }

            if (BattleCalculator.CanBattle(BattleSystem, Target))
            {
                var context = BattleAbilityContext.Create(BattleSystem, Target);
                BattleSystem.TryActivateAbility(BattleAbilityIds.Slash, ref context);
            }
            else
            {
                StateSystem.TryTransit(StateTags.Idle);
            }

            base.OnBattleTick();
        }

        protected override void OnChase()
        {
            Actor.SetDestination(ChasePosition);
            base.OnChase();
        }

        protected override void OnDie()
        {
            this.Log();
            Actor.Die();
            base.OnDie();
        }

        protected override void OnTerminated()
        {
            Actor.Die();
            base.OnTerminated();
        }

        private void OnHealthStatChanged()
        {
            SetHealth();
        }

        private void OnAttackStatChanged() { }

        private void OnDefenseStatChanged() { }

        private void OnMoveSpeedStatChanged()
        {
            var speed = BattleSystem.Stat.RateValue(StatId.MoveSpeed);
            Actor.SetSpeed(speed);
        }

        private void OnAttackSpeedStatChanged() { }

        private void OnCriticalRateStatChanged() { }

        private void OnCriticalDamageStatChanged() { }

        private void OnAttackRangeStatChanged()
        {
            var attackRange = BattleSystem.Stat.RateValue(StatId.AttackRange);
            Actor.SetAttackRange(attackRange);
        }

        private void OnDetectionRangeStatChanged()
        {
            var range = BattleSystem.Stat.RateValue(StatId.DetectionRange);
            Actor.SetDetectionRange(range);
        }

        private void OnHitRangeStatChanged()
        {
            var range = BattleSystem.Stat.RateValue(StatId.HitRange);
            Actor.SetHitRange(range);
        }

        private void OnHealthGaugeChanged()
        {
            SetHealth();
            if (BattleSystem.Gauge.Current(StatId.Health) <= 0)
            {
                StateSystem.DespawnAsync(1).Forget();
            }
        }

        private void SetHealth()
        {
            var cur = BattleSystem.Gauge.Current(StatId.Health);
            var max = BattleSystem.Stat.Value(StatId.Health);
            Actor.SetHealth(cur, max);
        }
    }
}