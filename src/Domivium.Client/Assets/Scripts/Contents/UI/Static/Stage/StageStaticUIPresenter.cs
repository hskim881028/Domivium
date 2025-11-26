using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Battle;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Audio;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Systems;
using Domivium.Client.Core.UI;
using Domivium.Client.Core.UI.Contract;
using Domivium.Client.Core.UI.Navigation;
using Domivium.Client.Core.UI.Presenter;
using Domivium.Client.Data.Stat;
using R3;
using UnityEngine;

namespace Domivium.Client.Contents.UI.Static
{
    public class StageStaticUIPresenter : StaticUIPresenter<StageStaticUIView, IStageStaticUIMessage>, IStageStaticUIMessage
    {
        private IBattleSystem _character;

        protected override HashSet<UILayer> Layer => UILayer.Set(UILayers.Stage);

        public override UIPriority Priority => UIPriorities.Stage;

        public StageStaticUIPresenter(
            StageStaticUIView view,
            IUINavigation navigation,
            IAudioPlayer audioPlayer,
            IActorManager actorManager,
            ILootSystem lootSystem)
            : base(view, navigation, audioPlayer)
        {
            actorManager.Character.Subscribe(OnChangeCharacter).AddTo(ref DisposableBag);
            lootSystem.OnFind.Subscribe(OnFindProp).AddTo(ref DisposableBag);
        }

        public override async UniTask<bool> InitializeAsync(CancellationToken token)
        {
            if (!await base.InitializeAsync(token)) return false;

            View.SetAvoidButton(Constant.AvoidCooldown);
            View.SetInteractButton(false);
            return true;
        }

        public override async UniTask ShowAsync(CancellationToken token, UIParam param, bool immediately = false)
        {
            await base.ShowAsync(token, param, immediately);
            OnProjectileCapacityChanged();
        }

        private void OnActivateAbility(BattleAbilitySpec ability)
        {
            if (ability.Id == BattleAbilityIds.Reload)
            {
                var reloadSpeed = _character.Stat.RateValue(StatId.ReloadSpeed);
                View.Reload(reloadSpeed);
            }
            else if (ability.Id == BattleAbilityIds.CancelReload)
            {
                View.CancelReload();
            }

            else if (ability.Id == BattleAbilityIds.Avoid)
            {
                View.SetAvoidButton(ability.Cooldown);
            }
        }

        private void OnAppliedEffect(BattleEffectContext context) { }

        private void OnLookAt(Vector2 value)
        {
            View.SetAttackButton(value.sqrMagnitude > Constant.CanAttackRange);
        }

        private void SetStatGauge(StatId statId)
        {
            var cur = _character.Gauge.Current(statId);
            var max = _character.Gauge.Max(statId);
            View.SetStatGauge(statId, cur, max);
        }

        private void OnHealthChanged()
        {
            SetStatGauge(StatId.Health);
        }

        private void OnHungerChanged()
        {
            SetStatGauge(StatId.Hunger);
        }

        private void OnStaminaChanged()
        {
            SetStatGauge(StatId.Stamina);
        }

        private void OnSanityChanged()
        {
            SetStatGauge(StatId.Sanity);
        }

        private void OnProjectileCapacityChanged()
        {
            var cur = _character.Gauge.Current(StatId.ProjectileCapacity);
            var max = _character.Gauge.Max(StatId.ProjectileCapacity);
            View.SetProjectileCapacity(cur, max);
        }

        private void OnFindProp(ushort lootId)
        {
            View.SetInteractButton(lootId > 0);
        }

        private void OnChangeCharacter(IUnitPresenter character)
        {
            if (character == null) return;

            _character = character.BattleSystem;

            _character.OnAppliedEffect.Subscribe(OnAppliedEffect).AddTo(ref DisposableBag);
            _character.OnActivateAbility.Subscribe(OnActivateAbility).AddTo(ref DisposableBag);
            _character.LookAt.Subscribe(OnLookAt).AddTo(ref DisposableBag);

            _character.Gauge.AddListener(StatId.Health, OnHealthChanged);
            _character.Gauge.AddListener(StatId.Hunger, OnHungerChanged);
            _character.Gauge.AddListener(StatId.Stamina, OnStaminaChanged);
            _character.Gauge.AddListener(StatId.Sanity, OnSanityChanged);
            _character.Gauge.AddListener(StatId.ProjectileCapacity, OnProjectileCapacityChanged);
        }
    }
}