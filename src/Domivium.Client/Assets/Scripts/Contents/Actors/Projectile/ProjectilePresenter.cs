using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Actors.Contract;
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
    public class ProjectilePresenter : PawnPresenter<Projectile>
    {
        private ActorId _target;
        private Vector2 _direction;
        private Vector2 _spawnPosition;

        protected ProjectilePresenter(Projectile actor, ISystemFactory systemFactory)
            : base(actor, systemFactory) { }

        public override async UniTask SpawnAsync(CancellationToken token, ActorParam param)
        {
            await base.SpawnAsync(token, param);

            var p = param.As<ProjectileParams>();
            var projectile = p.ProjectileContext;
            BattleSystem.Initialize(Uid, projectile.ActorId, projectile.Id, projectile.RarityType, p.SpawnPosition);

            BattleSystem.Stat.Register(StatId.Durability, projectile.Durability, OnDurabilityStatChanged);
            BattleSystem.Stat.Register(StatId.Attack, projectile.Attack, OnAttackStatChanged);
            BattleSystem.Stat.Register(StatId.AttackRange, projectile.AttackRange, OnAttackRangeStatChanged);
            BattleSystem.Stat.Register(StatId.ProjectileSpeed, projectile.ProjectileSpeed, OnProjectileSpeedStatChanged);
            BattleSystem.Stat.Register(StatId.CriticalRate, projectile.CriticalRate, OnCriticalRateStatChanged);
            BattleSystem.Stat.Register(StatId.CriticalDamage, projectile.CriticalDamage, OnCriticalDamageStatChanged);

            BattleSystem.Gauge.Register(StatId.Durability, projectile.Durability);
            BattleSystem.Gauge.AddListener(StatId.Health, OnDurabilityGaugeChanged);

            var sourceStatSet = p.SourceStatSet;
            BattleSystem.Stat.Apply(StatId.Attack, sourceStatSet.RawValue(StatId.Attack), StatChannel.Add);
            BattleSystem.Stat.Apply(StatId.AttackRange, sourceStatSet.RawValue(StatId.AttackRange), StatChannel.Add);
            BattleSystem.Stat.Apply(StatId.CriticalRate, sourceStatSet.RawValue(StatId.CriticalRate), StatChannel.Add);
            BattleSystem.Stat.Apply(StatId.CriticalDamage, sourceStatSet.RawValue(StatId.CriticalDamage), StatChannel.Add);

            foreach (var ability in p.Abilities)
            {
                BattleSystem.GrantAbility(ability);
            }

            _target = p.Target;
            _direction = p.Direction;
            _spawnPosition = p.SpawnPosition;
        }

        protected override void OnMoveTick(float deltaTime)
        {
            base.OnMoveTick(deltaTime);
            var dist = Vector2.Distance(_spawnPosition, BattleSystem.Position);
            var range = BattleSystem.Stat.RateValue(StatId.AttackRange);
            if (dist > range)
            {
                StateSystem.DespawnAsync().Forget();
                return;
            }

            var context = BattleAbilityContext.Create(BattleAbilityIds.Tracking, BattleSystem, _target, deltaTime);
            BattleSystem.TryActivateAbility(ref context);
        }
        
        

        protected override void OnIdle()
        {
            base.OnIdle();

            var context = BattleAbilityContext.Create(BattleAbilityIds.Turn, BattleSystem, _direction);
            if (!BattleSystem.TryActivateAbility(ref context))
            {
                throw new Exception("Failed to activate ability");
            }

            StateSystem.Transit(StateTags.Move);
        }

        protected virtual void OnDurabilityStatChanged() { }
        protected virtual void OnAttackStatChanged() { }
        protected virtual void OnAttackRangeStatChanged() { }
        protected virtual void OnProjectileSpeedStatChanged() { }
        protected virtual void OnCriticalRateStatChanged() { }
        protected virtual void OnCriticalDamageStatChanged() { }

        protected virtual void OnDurabilityGaugeChanged()
        {
            if (BattleSystem.Gauge.Current(StatId.Durability) <= 0)
            {
                this.Log();
                StateSystem.DespawnAsync().Forget();
            }
        }
    }
}