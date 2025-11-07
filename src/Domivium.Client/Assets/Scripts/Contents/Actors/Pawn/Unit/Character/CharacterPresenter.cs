using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Actors.Contract;
using Domivium.Client.Contents.Battle;
using Domivium.Client.Contents.State;
using Domivium.Client.Core.Actors.Contract;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Factory;
using Domivium.Client.Core.Systems;
using Domivium.Client.Data.Stat;
using R3;
using UnityEngine;

namespace Domivium.Client.Contents.Actors
{
    public class CharacterPresenter : UnitPresenter<Character>
    {
        private bool _firing;
        public Transform Transform => Actor.transform;

        public CharacterPresenter(
            Character actor,
            ISystemFactory systemFactory,
            ICharacterSystem characterSystem)
            : base(actor, systemFactory)
        {
            characterSystem.OnTurn.Subscribe(OnTurn).AddTo(ref DisposableBag);
            characterSystem.OnLookAt.Subscribe(OnLookAt).AddTo(ref DisposableBag);
            characterSystem.OnAvoid.Subscribe(OnAvoid).AddTo(ref DisposableBag);
            characterSystem.OnBattleTag.Subscribe(OnBattleTag).AddTo(ref DisposableBag);
        }

        public override async UniTask SpawnAsync(CancellationToken token, ActorParam param)
        {
            await base.SpawnAsync(token, param);

            var p = param.As<CharacterParams>();

            var wp = p.WeaponContext;

            BattleSystem.Stat.Apply(StatId.ProjectileCapacity, wp.ProjectileCapacity, StatChannel.Add);
            BattleSystem.Stat.Apply(StatId.Attack, wp.Attack, StatChannel.Add);
            BattleSystem.Stat.Apply(StatId.AttackRange, wp.AttackRange, StatChannel.Add);
            BattleSystem.Stat.Apply(StatId.AttackSpeed, wp.AttackSpeed, StatChannel.Add);
            BattleSystem.Stat.Apply(StatId.ReloadSpeed, wp.ReloadSpeed, StatChannel.Add);
            BattleSystem.Stat.Apply(StatId.CriticalRate, wp.CriticalRate, StatChannel.Add);
            BattleSystem.Stat.Apply(StatId.CriticalDamage, wp.CriticalDamage, StatChannel.Add);

            var projectileCapacity = BattleSystem.Stat.Value(StatId.ProjectileCapacity);
            BattleSystem.Gauge.Apply(StatId.ProjectileCapacity, projectileCapacity, GaugeChannel.Max);

            var attackSpeed = BattleSystem.Stat.RateValue(StatId.AttackSpeed);
            BattleSystem.SetAbilityCooldown(BattleAbilityIds.Attack, attackSpeed);
            BattleSystem.SetAbilityCooldown(BattleAbilityIds.Avoid, Constant.AvoidCooldown);
        }

        protected override void OnPostStateTick(float deltaTime)
        {
            base.OnPostStateTick(deltaTime);
            
            if (!_firing) return;

            var context = BattleAbilityContext.Create(BattleAbilityIds.Attack, BattleSystem);
            BattleSystem.TryActivateAbility(ref context);
        }

        protected override void OnMoveTick(float deltaTime)
        {
            base.OnMoveTick(deltaTime);
            var context = BattleAbilityContext.Create(BattleAbilityIds.Move, BattleSystem, deltaTime);
            if (!BattleSystem.TryActivateAbility(ref context)) return;

            var lookAt = BattleSystem.LookAt.CurrentValue;
            if (Mathf.Approximately(lookAt.sqrMagnitude, 0)) return;

            var attackRange = BattleSystem.Stat.RateValue(StatId.AttackRange);
            Actor.SetAim(lookAt, attackRange);
        }

        private void OnTurn(Vector2 value)
        {
            if (Mathf.Approximately(value.sqrMagnitude, 0))
            {
                StateSystem.Transit(StateTags.Idle);
                return;
            }

            var context = BattleAbilityContext.Create(BattleAbilityIds.Turn, BattleSystem, value);
            if (!BattleSystem.TryActivateAbility(ref context)) return;

            StateSystem.Transit(StateTags.Move);
        }

        private void OnLookAt(Vector2 value)
        {
            var context = BattleAbilityContext.Create(BattleAbilityIds.LookAt, BattleSystem, value);
            BattleSystem.TryActivateAbility(ref context);
        }

        protected override void OnLookAtChanged(Vector2 lookAt)
        {
            base.OnLookAtChanged(lookAt);
            if (Mathf.Approximately(lookAt.sqrMagnitude, 0))
            {
                Actor.HideAim();
            }
            else
            {
                var attackRange = BattleSystem.Stat.RateValue(StatId.AttackRange);
                Actor.SetAim(lookAt, attackRange);
            }
        }

        private void OnAvoid(R3.Unit unit)
        {
            var context = BattleAbilityContext.Create(BattleAbilityIds.Avoid, BattleSystem);
            BattleSystem.TryActivateAbility(ref context);
        }

        private void OnBattleTag(BattleTag tag)
        {
            _firing = tag == BattleTags.Firing;

            if (tag == BattleTags.Idle)
            {
                var context = BattleAbilityContext.Create(BattleAbilityIds.Reload, BattleSystem);
                BattleSystem.TryActivateAbility(ref context);
            }

            if (tag == BattleTags.Aiming || tag == BattleTags.Firing)
            {
                var context = BattleAbilityContext.Create(BattleAbilityIds.CancelReload, BattleSystem);
                BattleSystem.TryActivateAbility(ref context);
            }
        }
    }
}