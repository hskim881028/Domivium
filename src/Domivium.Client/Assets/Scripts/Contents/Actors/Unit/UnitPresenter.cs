using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Actors.Contract;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Actors.Contract;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Message;
using Domivium.Client.Data.Stat;
using MessagePipe;

namespace Domivium.Client.Contents.Actors
{
    public abstract class UnitPresenter<TUnit> : ActorPresenter<TUnit>, IUnitPresenter where TUnit : Unit
    {
        public BattleSystem BattleSystem { get; }

        protected UnitPresenter(Guid id, TUnit actor, IPublisher<ActorTagMessage> tagPublisher)
            : base(id, actor, tagPublisher)
        {
            BattleSystem = new BattleSystem(TagSet, Actor.transform);
        }

        public override UniTask ActivateAsync(CancellationToken token, ActorParam param)
        {
            var p = param.As<UnitParams>();
            var row = p.UnitContext;

            BattleSystem.Reset();

            foreach (var ability in p.Abilities)
            {
                BattleSystem.GrantAbility(ability);
            }

            BattleSystem.Stat.Register(StatId.Health, row.Health, OnHealthStatChanged);
            BattleSystem.Stat.Register(StatId.Attack, row.Attack, OnAttackStatChanged);
            BattleSystem.Stat.Register(StatId.Defense, row.Defense, OnDefenseStatChanged);
            BattleSystem.Stat.Register(StatId.AttackRange, row.AttackRange, OnAttackRangeStatChanged);
            BattleSystem.Stat.Register(StatId.DetectionRange, row.DetectionRange, OnDetectionRangeStatChanged);
            BattleSystem.Stat.Register(StatId.Speed, row.Speed, OnSpeedStatChanged);
            BattleSystem.Stat.Register(StatId.CriticalRate, row.CriticalRate, OnCriticalRateStatChanged);
            BattleSystem.Stat.Register(StatId.CriticalDamage, row.CriticalDamage, OnCriticalDamageStatChanged);

            BattleSystem.Gauge.Register(StatId.Health, row.Health, OnHealthGaugeChanged);
            return base.ActivateAsync(token, param);
        }

        public virtual void Tick(float deltaTime)
        {
            if (TagSet.State.CurrentValue == StateTag.Die || TagSet.State.CurrentValue == StateTag.Despawn) return;

            BattleSystem.Tick(deltaTime);
            Actor.Tick(deltaTime);
        }

        protected virtual void OnHealthStatChanged()
        {
            SetHealth();
        }

        protected virtual void OnAttackStatChanged() { }

        protected virtual void OnDefenseStatChanged() { }

        protected virtual void OnSpeedStatChanged() { }

        protected virtual void OnCriticalRateStatChanged() { }

        protected virtual void OnCriticalDamageStatChanged() { }

        protected virtual void OnAttackRangeStatChanged()
        {
            var attackRange = BattleSystem.Stat.Value(StatId.AttackRange);
            Actor.SetAttackRange(attackRange);
        }

        protected virtual void OnDetectionRangeStatChanged()
        {
            var range = BattleSystem.Stat.Value(StatId.DetectionRange);
            Actor.SetDetectionRange(range);
        }

        protected virtual void OnHealthGaugeChanged()
        {
            SetHealth();
        }

        private void SetHealth()
        {
            var cur = BattleSystem.Gauge.Current(StatId.Health);
            var max = BattleSystem.Stat.Value(StatId.Health);
            Actor.SetHealth(cur, max);
        }
    }
}