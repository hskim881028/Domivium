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

        protected ProjectilePresenter(Projectile actor, ISystemFactory systemFactory) : base(actor, systemFactory) { }

        public override async UniTask SpawnAsync(CancellationToken token, ActorParam param)
        {
            await base.SpawnAsync(token, param);

            var p = param.As<ProjectileParams>();
            var stat = p.SourceStatSet;
            BattleSystem.Initialize(Uid, ActorId.Projectile, p.SpawnPosition);

            BattleSystem.Stat.Register(StatId.Attack, stat.RawValue(StatId.Attack));
            BattleSystem.Stat.Register(StatId.Penetration, stat.RawValue(StatId.Penetration));
            BattleSystem.Stat.Register(StatId.AttackRange, stat.RawValue(StatId.AttackRange));
            BattleSystem.Stat.Register(StatId.ProjectileSpeed, stat.RawValue(StatId.ProjectileSpeed));
            BattleSystem.Stat.Register(StatId.CriticalRate, stat.RawValue(StatId.CriticalRate));
            BattleSystem.Stat.Register(StatId.CriticalDamage, stat.RawValue(StatId.CriticalDamage));

            BattleSystem.Stat.AddListener(StatId.Attack, OnAttackStatChanged);
            BattleSystem.Stat.AddListener(StatId.Penetration, OnPenetrationStatChanged);
            BattleSystem.Stat.AddListener(StatId.AttackRange, OnAttackRangeStatChanged);
            BattleSystem.Stat.AddListener(StatId.ProjectileSpeed, OnProjectileSpeedStatChanged);
            BattleSystem.Stat.AddListener(StatId.CriticalRate, OnCriticalRateStatChanged);
            BattleSystem.Stat.AddListener(StatId.CriticalDamage, OnCriticalDamageStatChanged);

            BattleSystem.Gauge.Register(StatId.Penetration, stat.RawValue(StatId.Penetration), stat.RawValue(StatId.Penetration));

            BattleSystem.Gauge.AddListener(StatId.Penetration, OnPenetrationGaugeChanged);

            foreach (var ability in p.Abilities)
            {
                BattleSystem.GrantAbility(ability);
            }

            _target = p.TargetActorId;
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
                Die();
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

        protected virtual void OnAttackStatChanged() { }
        protected virtual void OnPenetrationStatChanged() { }
        protected virtual void OnAttackRangeStatChanged() { }
        protected virtual void OnProjectileSpeedStatChanged() { }
        protected virtual void OnCriticalRateStatChanged() { }
        protected virtual void OnCriticalDamageStatChanged() { }

        protected virtual void OnPenetrationGaugeChanged()
        {
            var current = BattleSystem.Gauge.Current(StatId.Penetration);
            if (current <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            StateSystem.DespawnAsync().Forget();
        }
    }
}