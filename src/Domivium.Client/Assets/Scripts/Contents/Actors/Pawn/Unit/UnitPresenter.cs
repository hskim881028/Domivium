using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Actors.Contract;
using Domivium.Client.Contents.State;
using Domivium.Client.Core;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Actors.Contract;
using Domivium.Client.Core.Factory;
using Domivium.Client.Data.Stat;
using UnityEngine;

namespace Domivium.Client.Contents.Actors
{
    public abstract class UnitPresenter<TUnit> : PawnPresenter<TUnit>, IUnitPresenter where TUnit : Unit
    {
        protected UnitPresenter(TUnit actor, ISystemFactory systemFactory)
            : base(actor, systemFactory) { }

        public override async UniTask SpawnAsync(CancellationToken token, ActorParam param)
        {
            await base.SpawnAsync(token, param);

            var p = param.As<UnitParams>();
            var row = p.UnitContext;

            BattleSystem.Initialize(Uid, row.ActorId, p.SpawnPosition);

            BattleSystem.Stat.Register(StatId.Health, row.Health);
            BattleSystem.Stat.Register(StatId.Hunger, row.Hunger);
            BattleSystem.Stat.Register(StatId.Stamina, row.Stamina);
            BattleSystem.Stat.Register(StatId.Sanity, row.Sanity);
            BattleSystem.Stat.Register(StatId.Durability, row.Durability);
            BattleSystem.Stat.Register(StatId.Weight, row.Weight);
            BattleSystem.Stat.Register(StatId.InventoryCapacity, row.InventoryCapacity);
            BattleSystem.Stat.Register(StatId.ProjectileCapacity, row.ProjectileCapacity);
            BattleSystem.Stat.Register(StatId.Attack, row.Attack);
            BattleSystem.Stat.Register(StatId.Defense, row.Defense);
            BattleSystem.Stat.Register(StatId.Penetration, row.Penetration);
            BattleSystem.Stat.Register(StatId.AttackRange, row.AttackRange);
            BattleSystem.Stat.Register(StatId.DetectionRange, row.DetectionRange);
            BattleSystem.Stat.Register(StatId.MoveSpeed, row.MoveSpeed);
            BattleSystem.Stat.Register(StatId.AttackSpeed, row.AttackSpeed);
            BattleSystem.Stat.Register(StatId.ReloadSpeed, row.ReloadSpeed);
            BattleSystem.Stat.Register(StatId.ProjectileSpeed, row.ProjectileSpeed);
            BattleSystem.Stat.Register(StatId.CriticalRate, row.CriticalRate);
            BattleSystem.Stat.Register(StatId.CriticalDamage, row.CriticalDamage);

            BattleSystem.Stat.AddListener(StatId.Health, OnHealthStatChanged);
            BattleSystem.Stat.AddListener(StatId.Hunger, OnHungerStatChanged);
            BattleSystem.Stat.AddListener(StatId.Stamina, OnStaminaStatChanged);
            BattleSystem.Stat.AddListener(StatId.Sanity, OnSanityStatChanged);
            BattleSystem.Stat.AddListener(StatId.Durability, OnDurabilityStatChanged);
            BattleSystem.Stat.AddListener(StatId.Weight, OnWeightStatChanged);
            BattleSystem.Stat.AddListener(StatId.WeightCapacity, OnWeightCapacityStatChanged);
            BattleSystem.Stat.AddListener(StatId.InventoryCapacity, OnInventoryCapacityStatChanged);
            BattleSystem.Stat.AddListener(StatId.ProjectileCapacity, OnProjectileCapacityStatChanged);
            BattleSystem.Stat.AddListener(StatId.Attack, OnAttackStatChanged);
            BattleSystem.Stat.AddListener(StatId.Defense, OnDefenseStatChanged);
            BattleSystem.Stat.AddListener(StatId.Penetration, OnPenetrationStatChanged);
            BattleSystem.Stat.AddListener(StatId.AttackRange, OnAttackRangeStatChanged);
            BattleSystem.Stat.AddListener(StatId.DetectionRange, OnDetectionRangeStatChanged);
            BattleSystem.Stat.AddListener(StatId.MoveSpeed, OnMoveSpeedStatChanged);
            BattleSystem.Stat.AddListener(StatId.AttackSpeed, OnAttackSpeedStatChanged);
            BattleSystem.Stat.AddListener(StatId.ReloadSpeed, OnReloadSpeedStatChanged);
            BattleSystem.Stat.AddListener(StatId.ProjectileSpeed, OnProjectileSpeedStatChanged);
            BattleSystem.Stat.AddListener(StatId.CriticalRate, OnCriticalRateStatChanged);
            BattleSystem.Stat.AddListener(StatId.CriticalDamage, OnCriticalDamageStatChanged);

            BattleSystem.Gauge.Register(StatId.Health, row.Health, row.Health);
            BattleSystem.Gauge.Register(StatId.Hunger, row.Hunger, row.Hunger);
            BattleSystem.Gauge.Register(StatId.Stamina, row.Stamina, row.Stamina);
            BattleSystem.Gauge.Register(StatId.Sanity, row.Sanity, row.Sanity);
            BattleSystem.Gauge.Register(StatId.WeightCapacity, 0, row.Sanity);
            BattleSystem.Gauge.Register(StatId.InventoryCapacity, 0, row.InventoryCapacity);
            BattleSystem.Gauge.Register(StatId.ProjectileCapacity, 0, row.ProjectileCapacity);

            BattleSystem.Gauge.AddListener(StatId.Health, OnHealthGaugeChanged);
            BattleSystem.Gauge.AddListener(StatId.Hunger, OnHungerGaugeChanged);
            BattleSystem.Gauge.AddListener(StatId.Stamina, OnStaminaGaugeChanged);
            BattleSystem.Gauge.AddListener(StatId.Sanity, OnSanityGaugeChanged);
            BattleSystem.Gauge.AddListener(StatId.WeightCapacity, OnWeightCapacityGaugeChanged);
            BattleSystem.Gauge.AddListener(StatId.InventoryCapacity, OnInventoryCapacityGaugeChanged);
            BattleSystem.Gauge.AddListener(StatId.ProjectileCapacity, OnProjectileCapacityGaugeChanged);

            foreach (var ability in p.Abilities)
            {
                BattleSystem.GrantAbility(ability);
            }

            StateSystem.Transit(StateTags.Idle);
        }

        #region Stat

        protected virtual void OnHealthStatChanged()
        {
            SetHealth();
        }

        protected virtual void OnHungerStatChanged() { }

        protected virtual void OnStaminaStatChanged() { }

        protected virtual void OnSanityStatChanged() { }

        protected virtual void OnDurabilityStatChanged() { }

        protected virtual void OnWeightStatChanged() { }

        protected virtual void OnWeightCapacityStatChanged() { }

        protected virtual void OnInventoryCapacityStatChanged() { }

        protected virtual void OnProjectileCapacityStatChanged() { }

        protected virtual void OnAttackStatChanged() { }

        protected virtual void OnDefenseStatChanged() { }

        protected virtual void OnPenetrationStatChanged() { }

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

        protected virtual void OnMoveSpeedStatChanged() { }
        protected virtual void OnAttackSpeedStatChanged() { }
        protected virtual void OnReloadSpeedStatChanged() { }
        protected virtual void OnProjectileSpeedStatChanged() { }
        protected virtual void OnCriticalRateStatChanged() { }
        protected virtual void OnCriticalDamageStatChanged() { }

        #endregion

        #region Gauge

        protected virtual void OnHealthGaugeChanged()
        {
            SetHealth();
            var curHp = BattleSystem.Gauge.Current(StatId.Health);
            if (curHp <= 0)
            {
                StateSystem.DespawnAsync(1).Forget();
            }
        }

        protected virtual void OnHungerGaugeChanged() { }
        protected virtual void OnStaminaGaugeChanged() { }
        protected virtual void OnSanityGaugeChanged() { }
        protected virtual void OnWeightCapacityGaugeChanged() { }
        protected virtual void OnInventoryCapacityGaugeChanged() { }
        protected virtual void OnProjectileCapacityGaugeChanged() { }

        #endregion

        private void SetHealth()
        {
            var cur = BattleSystem.Gauge.Current(StatId.Health);
            var max = BattleSystem.Stat.Value(StatId.Health);
            Actor.SetHealth(cur, max);
        }
    }
}