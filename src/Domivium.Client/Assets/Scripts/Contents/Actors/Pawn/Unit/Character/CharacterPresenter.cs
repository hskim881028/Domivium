using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Actors.Contract;
using Domivium.Client.Contents.Battle;
using Domivium.Client.Contents.Services;
using Domivium.Client.Contents.State;
using Domivium.Client.Core.Actors.Contract;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Factory;
using Domivium.Client.Core.Systems;
using Domivium.Client.Data.Item;
using Domivium.Client.Data.Stat;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Domivium.Client.Contents.Actors
{
    public class CharacterPresenter : UnitPresenter<Character>
    {
        private readonly MasterDbService _masterDbService;
        private readonly IItemSystem _itemSystem;
        private bool _firing;

        public CharacterPresenter(
            Character actor,
            ISystemFactory systemFactory,
            MasterDbService masterDbService,
            IItemSystem itemSystem,
            ICharacterSystem characterSystem)
            : base(actor, systemFactory)
        {
            _masterDbService = masterDbService;
            _itemSystem = itemSystem;
            _itemSystem.Equipment.CollectionChanged += OnChangedEquipment;
            _itemSystem.LoadedProjectile.Subscribe(OnLoadedProjectile).AddTo(ref DisposableBag);
            _itemSystem.TotalProjectile.Subscribe(OnTotalProjectile).AddTo(ref DisposableBag);
            _itemSystem.TotalWeight.Subscribe(TotalWeight).AddTo(ref DisposableBag);

            characterSystem.OnTurn.Subscribe(OnTurn).AddTo(ref DisposableBag);
            characterSystem.OnLookAt.Subscribe(OnLookAt).AddTo(ref DisposableBag);
            characterSystem.OnBattleTag.Subscribe(OnBattleTag).AddTo(ref DisposableBag);
        }

        public override async UniTask SpawnAsync(CancellationToken token, ActorParam param)
        {
            await base.SpawnAsync(token, param);
            Actor.SetSight(param is not LobbyCharacterParams);
        }

        protected override void OnDispose()
        {
            _itemSystem.Equipment.CollectionChanged -= OnChangedEquipment;
            base.OnDispose();
        }

        protected override void OnPostStateTick(float deltaTime)
        {
            base.OnPostStateTick(deltaTime);

            OnFindLoot();
            OnFiringTick();
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

        protected override void OnLookAtChanged(Vector2 lookAt)
        {
            base.OnLookAtChanged(lookAt);
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

            if (tag == BattleTags.Avoid)
            {
                var context = BattleAbilityContext.Create(BattleAbilityIds.Avoid, BattleSystem);
                BattleSystem.TryActivateAbility(ref context);
            }
        }

        private void OnFindLoot() { }

        private void OnFiringTick()
        {
            if (!_firing) return;

            var context = BattleAbilityContext.Create(BattleAbilityIds.Attack, BattleSystem);
            BattleSystem.TryActivateAbility(ref context);
        }

        private void OnChangedEquipment(in NotifyCollectionChangedEventArgs<KeyValuePair<int, ItemData>> e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    ApplyStat(e.NewItem.Value);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    ApplyStat(e.OldItem.Value, true);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    ApplyStat(e.OldItem.Value, true);
                    ApplyStat(e.NewItem.Value);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                case NotifyCollectionChangedAction.Move:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ApplyStat(ItemData item, bool unequip = false)
        {
            var mul = unequip ? -1 : 1;
            switch (item.Type)
            {
                case ItemType.Weapon:
                    ApplyWeaponStat(item, mul);
                    break;
                case ItemType.Helmet:
                    break;
                case ItemType.Necklace:
                    break;
                case ItemType.Backpack:
                    break;
                case ItemType.Projectile:
                    ApplyProjectileStat(item, mul);
                    break;
                case ItemType.Armor:
                    break;
                case ItemType.Ring:
                    break;
                case ItemType.Food: //  do nothing
                case ItemType.Potion: //  do nothing
                    break;
                case ItemType.None:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ApplyWeaponStat(ItemData item, int mul)
        {
            if (!_masterDbService.DB.WeaponRowTable.TryFindById(item.Id, out var wp)) return;

            BattleSystem.Stat.Apply(StatId.ProjectileCapacity, wp.ProjectileCapacity * mul, StatChannel.Add);
            BattleSystem.Stat.Apply(StatId.Attack, wp.Attack * mul, StatChannel.Add);
            BattleSystem.Stat.Apply(StatId.AttackRange, wp.AttackRange * mul, StatChannel.Add);
            BattleSystem.Stat.Apply(StatId.AttackSpeed, wp.AttackSpeed * mul, StatChannel.Add);
            BattleSystem.Stat.Apply(StatId.ReloadSpeed, wp.ReloadSpeed * mul, StatChannel.Add);
            BattleSystem.Stat.Apply(StatId.CriticalRate, wp.CriticalRate * mul, StatChannel.Add);
            BattleSystem.Stat.Apply(StatId.CriticalDamage, wp.CriticalDamage * mul, StatChannel.Add);
        }

        private void ApplyProjectileStat(ItemData item, int mul)
        {
            if (!_masterDbService.DB.ProjectileRowTable.TryFindById(item.Id, out var proj)) return;

            BattleSystem.Stat.Apply(StatId.Attack, proj.Attack * mul, StatChannel.Add);
            BattleSystem.Stat.Apply(StatId.ProjectileSpeed, proj.AttackSpeed * mul, StatChannel.Add);
            BattleSystem.Stat.Apply(StatId.CriticalRate, proj.CriticalRate * mul, StatChannel.Add);
            BattleSystem.Stat.Apply(StatId.CriticalDamage, proj.CriticalDamage * mul, StatChannel.Add);
        }

        private void OnLoadedProjectile(int count)
        {
            BattleSystem.Gauge.Apply(StatId.ProjectileCapacity, count, GaugeChannel.Set);
        }

        private void OnTotalProjectile(int count)
        {
            BattleSystem.Gauge.ApplyMax(StatId.ProjectileCapacity, count, GaugeChannel.Set);
        }

        private void TotalWeight(int weight)
        {
            BattleSystem.Gauge.Apply(StatId.Weight, weight, GaugeChannel.Set);
        }
    }
}