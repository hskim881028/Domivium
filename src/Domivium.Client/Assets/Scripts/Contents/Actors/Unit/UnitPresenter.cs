using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Actors.Contract;
using Domivium.Client.Contents.Battle;
using Domivium.Client.Contents.ReadModels;
using Domivium.Client.Contents.State;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Actors.Contract;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Factory;
using Domivium.Client.Data.Stat;
using R3;
using UnityEngine;

namespace Domivium.Client.Contents.Actors
{
    public abstract class UnitPresenter<TUnit> : ActorPresenter<TUnit>, IUnitPresenter where TUnit : Unit
    {
        private const float UpdateDistance = 0.5f;
        private const float Hysteresis = 0.25f;
        private const float HysteresisSq = Hysteresis * Hysteresis;

        protected readonly IBattleService BattleService;
        protected IBattleSystem Target;
        protected Vector3 ChasePosition;
        protected ActorId TargetActionId;
        protected BattleAbilityId BattleAbilityId;
        protected bool LockOn;

        public IBattleSystem BattleSystem { get; }

        protected UnitPresenter(
            TUnit actor,
            ISystemFactory systemFactory,
            IBattleService battleService)
            : base(actor, systemFactory)
        {
            BattleSystem = systemFactory.CreateBattle(actor.transform, StateSystem.Tag);
            BattleSystem.AppliedEffect.Subscribe(OnAppliedEffectChanged).AddTo(ref DisposableBag);
            BattleService = battleService;
        }

        public override UniTask ActivateAsync(CancellationToken token, ActorParam param)
        {
            var p = param.As<UnitParams>();
            var row = p.UnitContext;

            TargetActionId = row.TargetActionId;
            BattleAbilityId = row.BattleAbilityId;

            BattleSystem.Initialize(Id, row.ActorId, row.UnitType);

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

            StateSystem.Spawn();
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

            if (BattleService.RecalculateChasePosition(BattleSystem, Target, out var chasePosition))
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
                var context = BattleAbilityContext.Create(BattleAbilityId, BattleSystem, Target);
                BattleSystem.TryActivateAbility(ref context);
            }
            else
            {
                StateSystem.TryTransit(StateTags.Chase);
            }

            base.OnBattleTick();
        }

        protected override void OnIdle()
        {
            LockOn = false;
            base.OnIdle();
        }

        protected override void OnChase()
        {
            Actor.SetDestination(ChasePosition);
            base.OnChase();
        }

        protected override void OnBattle()
        {
            LockOn = true;
            var offset = BattleService.GetPositionOffset(BattleSystem, Target);
            Actor.Battle(offset);
            base.OnBattle();
        }

        protected override void OnMove()
        {
            LockOn = false;
            base.OnMove();
        }

        protected override void OnDie()
        {
            LockOn = false;
            Target = null;
            ChasePosition = Vector3.zero;
            this.Log();
            Actor.Die();
            base.OnDie();
        }

        protected override void OnTerminated()
        {
            LockOn = false;
            Actor.Die();
            base.OnTerminated();
        }

        private void OnAppliedEffectChanged(BattleEffectContext context)
        {
            if (context.EffectId == BattleEffectIds.Damage)
            {
                OnDamagedEffect(context);
            }
        }

        protected virtual void OnDamagedEffect(BattleEffectContext context) { }

        protected virtual bool CheckForceSwapTarget() => false;

        protected void CheckSwapTarget(BattleEffectContext context)
        {
            if (Target.Id == context.Source.Id) return;

            if (!BattleService.TryGetChasePosition(BattleSystem, context.Source, out var chasePosition)) return;

            if (CheckForceSwapTarget())
            {
                LockOnTarget(context.Source, chasePosition);
                return;
            }

            if (StateSystem.Tag.CurrentValue == StateTags.Battle)
            {
                var newDistSq = (Actor.transform.position - chasePosition).sqrMagnitude;
                var curDistSq = (Actor.transform.position - ChasePosition).sqrMagnitude;
                if (newDistSq + HysteresisSq > curDistSq) return;
            }
            else
            {
                if (LockOn) return;
            }

            LockOnTarget(context.Source, chasePosition);
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

        private void LockOnTarget(IBattleSystem battleSystem, Vector3 chasePosition)
        {
            Target = battleSystem;
            LockOn = true;
            ChasePosition = chasePosition;
        }
    }
}