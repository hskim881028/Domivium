using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Actors.Contract;
using Domivium.Client.Contents.Battle;
using Domivium.Client.Contents.State;
using Domivium.Client.Core.Actors.Contract;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Factory;
using Domivium.Client.Data.Stat;
using UnityEngine;

namespace Domivium.Client.Contents.Actors
{
    public class MonsterPresenter : UnitPresenter<Monster>
    {
        private BattleTag _tag;
        private IBattleSystem _target;

        public MonsterPresenter(Monster actor, ISystemFactory systemFactory) : base(actor, systemFactory) { }

        public override async UniTask SpawnAsync(CancellationToken token, ActorParam param)
        {
            await base.SpawnAsync(token, param);

            var p = param.As<MonsterParams>();
            _target = p.Target;
        }

        protected override void OnIdle()
        {
            base.OnIdle();
            _tag = BattleTags.Idle;
        }

        protected override void OnMoveTick(float deltaTime)
        {
            base.OnMoveTick(deltaTime);
            var attackRange = BattleSystem.Stat.RateValue(StatId.AttackRange);
            var targetDistance = Vector2.Distance(_target.Position, BattleSystem.Position);
            if (targetDistance < attackRange)
            {
                var projectileCapacity = BattleSystem.Gauge.Current(StatId.ProjectileCapacity);
                _tag = projectileCapacity > 0 ? BattleTags.Firing : BattleTags.Reloading;
            }
            else
            {
                if (_tag == BattleTags.Reloading) return;

                _tag = BattleTags.Aiming;
            }
        }

        protected override void OnPostStateTick(float deltaTime)
        {
            base.OnPostStateTick(deltaTime);

            var detectDist = Vector2.Distance(_target.Position, BattleSystem.Position);
            var detectionRange = BattleSystem.Stat.RateValue(StatId.DetectionRange);
            StateSystem.Transit(detectDist < detectionRange ? StateTags.Move : StateTags.Idle);

            // this.Log(_tag.ToName());
            if (_tag == BattleTags.Idle) { }

            if (_tag == BattleTags.Aiming)
            {
                var context = BattleAbilityContext.Create(BattleAbilityIds.Chase, BattleSystem, _target.Position, deltaTime);
                BattleSystem.TryActivateAbility(ref context);
            }

            if (_tag == BattleTags.Firing)
            {
                var lookAt = _target.Position - BattleSystem.Position;
                lookAt.Normalize();
                var lookAtContext = BattleAbilityContext.Create(BattleAbilityIds.LookAt, BattleSystem, lookAt);
                BattleSystem.TryActivateAbility(ref lookAtContext);

                var context = BattleAbilityContext.Create(BattleAbilityIds.Attack, BattleSystem);
                if (BattleSystem.TryActivateAbility(ref context))
                {
                    Actor.ShowAsync().Forget();
                }
            }

            if (_tag == BattleTags.Reloading)
            {
                var context = BattleAbilityContext.Create(BattleAbilityIds.Reload, BattleSystem);
                BattleSystem.TryActivateAbility(ref context);
            }
        }

        protected override void OnHealthGaugeChanged()
        {
            base.OnHealthGaugeChanged();
            Actor.ShowAsync().Forget();
        }
    }
}