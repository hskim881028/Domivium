using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Actors.Contract;
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

            BattleSystem.Initialize(Uid, row.ActorId, row.Id, row.RarityType, p.SpawnPosition);

            BattleSystem.Stat.Register(StatId.Health, row.Health, OnHealthStatChanged);
            BattleSystem.Stat.Register(StatId.Hunger, row.Hunger, OnHungerStatChanged);
            BattleSystem.Stat.Register(StatId.Stamina, row.Stamina, OnStaminaStatChanged);
            BattleSystem.Stat.Register(StatId.Sanity, row.Sanity, OnSanityStatChanged);
            BattleSystem.Stat.Register(StatId.Durability, row.Durability, OnDurabilityStatChanged);
            BattleSystem.Stat.Register(StatId.Weight, row.Weight, OnWeightStatChanged);
            BattleSystem.Stat.Register(StatId.InventoryCapacity, row.InventoryCapacity, OnInventoryCapacityStatChanged);
            BattleSystem.Stat.Register(StatId.ProjectileCapacity, row.ProjectileCapacity, OnProjectileCapacityStatChanged);
            BattleSystem.Stat.Register(StatId.Attack, row.Attack, OnAttackStatChanged);
            BattleSystem.Stat.Register(StatId.Defense, row.Defense, OnDefenseStatChanged);
            BattleSystem.Stat.Register(StatId.AttackRange, row.AttackRange, OnAttackRangeStatChanged);
            BattleSystem.Stat.Register(StatId.DetectionRange, row.DetectionRange, OnDetectionRangeStatChanged);
            BattleSystem.Stat.Register(StatId.MoveSpeed, row.MoveSpeed, OnMoveSpeedStatChanged);
            BattleSystem.Stat.Register(StatId.AttackSpeed, row.AttackSpeed, OnAttackSpeedStatChanged);
            BattleSystem.Stat.Register(StatId.ReloadSpeed, row.ReloadSpeed, OnReloadSpeedStatChanged);
            BattleSystem.Stat.Register(StatId.ProjectileSpeed, row.ProjectileSpeed, OnProjectileSpeedStatChanged);
            BattleSystem.Stat.Register(StatId.CriticalRate, row.CriticalRate, OnCriticalRateStatChanged);
            BattleSystem.Stat.Register(StatId.CriticalDamage, row.CriticalDamage, OnCriticalDamageStatChanged);

            BattleSystem.Gauge.Register(StatId.Health, row.Health);
            BattleSystem.Gauge.Register(StatId.Hunger, row.Hunger);
            BattleSystem.Gauge.Register(StatId.Stamina, row.Stamina);
            BattleSystem.Gauge.Register(StatId.Sanity, row.Sanity);
            BattleSystem.Gauge.Register(StatId.Weight, row.Weight);
            BattleSystem.Gauge.Register(StatId.InventoryCapacity, row.InventoryCapacity);
            BattleSystem.Gauge.Register(StatId.ProjectileCapacity, row.ProjectileCapacity);

            BattleSystem.Gauge.AddListener(StatId.Health, OnHealthGaugeChanged);
            BattleSystem.Gauge.AddListener(StatId.Hunger, OnHungerGaugeChanged);
            BattleSystem.Gauge.AddListener(StatId.Stamina, OnStaminaGaugeChanged);
            BattleSystem.Gauge.AddListener(StatId.Sanity, OnSanityGaugeChanged);
            BattleSystem.Gauge.AddListener(StatId.Weight, OnWeightGaugeChanged);
            BattleSystem.Gauge.AddListener(StatId.InventoryCapacity, OnInventoryCapacityGaugeChanged);
            BattleSystem.Gauge.AddListener(StatId.ProjectileCapacity, OnProjectileCapacityGaugeChanged);

            foreach (var ability in p.Abilities)
            {
                BattleSystem.GrantAbility(ability);
            }
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

        protected virtual void OnMaxWeightStatChanged() { }

        protected virtual void OnInventoryCapacityStatChanged() { }

        protected virtual void OnProjectileCapacityStatChanged() { }

        protected virtual void OnAttackStatChanged() { }

        protected virtual void OnDefenseStatChanged() { }

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
        protected virtual void OnWeightGaugeChanged() { }
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