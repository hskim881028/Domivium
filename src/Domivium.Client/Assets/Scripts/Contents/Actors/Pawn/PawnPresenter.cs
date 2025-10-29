using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Actors.Contract;
using Domivium.Client.Contents.Battle;
using Domivium.Client.Contents.System.Model;
using Domivium.Client.Core;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Actors.Contract;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Factory;
using Domivium.Client.Data.Stat;
using R3;
using UnityEngine;

namespace Domivium.Client.Contents.Actors
{
    public abstract class PawnPresenter<TPawn> : ActorPresenter<TPawn>, IPawnPresenter where TPawn : Pawn
    {
        protected readonly IStageSystemModel StageSystemModel;

        public IBattleSystem BattleSystem { get; }

        protected PawnPresenter(TPawn actor, ISystemFactory systemFactory, IStageSystemModel stageSystemModel)
            : base(actor, systemFactory)
        {
            BattleSystem = systemFactory.CreateBattle(actor, StateSystem.Tag);
            BattleSystem.AppliedEffect.Subscribe(OnAppliedEffectChanged).AddTo(ref DisposableBag);
            BattleSystem.IsRight.Subscribe(Actor.SetFlip).AddTo(ref DisposableBag);
            StageSystemModel = stageSystemModel;
        }

        public override UniTask ActivateAsync(CancellationToken token, ActorParam param)
        {
            var p = param.As<UnitParams>();
            var row = p.PawnContext;

            BattleSystem.Initialize(Uid, row.ActorId, row.Id, row.PawnType, row.PawnRarityType);

            BattleSystem.Stat.Register(StatId.Level, 1, OnLevelStatChanged);
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

            StateSystem.Activate();
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

        protected virtual void OnDamagedEffect(BattleEffectContext context) { }

        protected virtual void OnLevelStatChanged()
        {
            SetHealth();
        }

        protected virtual void OnHealthStatChanged()
        {
            SetHealth();
        }

        protected virtual void OnAttackStatChanged() { }

        protected virtual void OnDefenseStatChanged() { }

        protected virtual void OnMoveSpeedStatChanged() { }

        protected virtual void OnAttackSpeedStatChanged() { }

        protected virtual void OnCriticalRateStatChanged() { }

        protected virtual void OnCriticalDamageStatChanged() { }

        protected virtual void OnAttackRangeStatChanged()
        {
            if (!AppEnv.DrawRange) return;

            var range = BattleSystem.Stat.RateValue(StatId.AttackRange);
            Actor.SetRange(StatId.AttackRange, range, Color.crimson);
        }

        protected virtual void OnDetectionRangeStatChanged()
        {
            if (!AppEnv.DrawRange) return;

            var range = BattleSystem.Stat.RateValue(StatId.DetectionRange);
            Actor.SetRange(StatId.DetectionRange, range, Color.gold);
        }

        protected virtual void OnHitRangeStatChanged()
        {
            if (!AppEnv.DrawRange) return;

            var range = BattleSystem.Stat.RateValue(StatId.HitRange);
            Actor.SetRange(StatId.HitRange, range, Color.chartreuse);
        }

        protected virtual void OnHealthGaugeChanged()
        {
            SetHealth();
            if (BattleSystem.Gauge.Current(StatId.Health) <= 0)
            {
                StateSystem.DespawnAsync(1).Forget();
            }
        }

        private void OnAppliedEffectChanged(BattleEffectContext context)
        {
            if (context.EffectId == BattleEffectIds.Damage)
            {
                OnDamagedEffect(context);
            }
        }

        private void SetHealth()
        {
            return;

            var cur = BattleSystem.Gauge.Current(StatId.Health);
            var max = BattleSystem.Stat.Value(StatId.Health);
            Actor.SetHealth(cur, max);
        }
    }
}