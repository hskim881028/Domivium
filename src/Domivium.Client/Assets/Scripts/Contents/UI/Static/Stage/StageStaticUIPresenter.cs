using System.Collections.Generic;
using Domivium.Client.Contents.Battle;
using Domivium.Client.Contents.DI;
using Domivium.Client.Contents.Services;
using Domivium.Client.Core.Audio;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Systems;
using Domivium.Client.Core.UI;
using Domivium.Client.Core.UI.Navigation;
using Domivium.Client.Core.UI.Presenter;
using Domivium.Client.Data.Stat;
using R3;
using UnityEngine;

namespace Domivium.Client.Contents.UI.Static
{
    public class StageStaticUIPresenter : StaticUIPresenter<StageStaticUIView, IStageStaticUIMessage>, IStageStaticUIMessage
    {
        private readonly SceneService _sceneService;
        private readonly ICharacterSystem _characterSystem;
        private readonly ICameraSystem _cameraSystem;
        private IBattleSystem _character;

        protected override HashSet<UILayer> Layer => UILayer.Set(UILayers.Stage);

        public override UIPriority Priority => UIPriorities.Stage;

        public StageStaticUIPresenter(
            StageStaticUIView view,
            IUINavigation navigation,
            IAudioPlayer audioPlayer,
            SceneService sceneService,
            ICharacterSystem characterSystem)
            : base(view, navigation, audioPlayer)
        {
            _sceneService = sceneService;
            characterSystem.OnInitialize.Subscribe(OnInitialize).AddTo(ref DisposableBag);
            characterSystem.OnLookAt.Subscribe(OnLookAt).AddTo(ref DisposableBag);
            characterSystem.OnBattleTag.Subscribe(OnBattleTag).AddTo(ref DisposableBag);
        }

        public void EnterLobby()
        {
            _sceneService.Load(SceneScopeIds.Lobby);
        }

        private void OnInitialize(IBattleSystem character)
        {
            _character = character;
            _character.Gauge.AddListener(StatId.Health, OnHealthChanged);
            _character.Gauge.AddListener(StatId.Hunger, OnHungerChanged);
            _character.Gauge.AddListener(StatId.Stamina, OnStaminaChanged);
            _character.Gauge.AddListener(StatId.Sanity, OnSanityChanged);
            _character.Gauge.AddListener(StatId.ProjectileCapacity, OnProjectileCapacityChanged);
            View.SetAvoidButton(Constant.AvoidCooldown);
        }

        private void OnLookAt(Vector2 value)
        {
            View.SetAttackButton(value.sqrMagnitude > Constant.CanAttackRange);
        }

        private void OnHealthChanged()
        {
            var cur = _character.Gauge.Current(StatId.Health);
            var max = _character.Stat.Value(StatId.Health);
            View.SetHealth(cur, max);
        }

        private void OnHungerChanged()
        {
            var cur = _character.Gauge.Current(StatId.Hunger);
            var max = _character.Stat.Value(StatId.Hunger);
            View.SetHunger(cur, max);
        }

        private void OnStaminaChanged()
        {
            var cur = _character.Gauge.Current(StatId.Stamina);
            var max = _character.Stat.Value(StatId.Stamina);
            View.SetStamina(cur, max);
        }

        private void OnSanityChanged()
        {
            var cur = _character.Gauge.Current(StatId.Sanity);
            var max = _character.Stat.Value(StatId.Sanity);
            View.SetSanity(cur, max);
        }

        private void OnProjectileCapacityChanged()
        {
            var cur = _character.Gauge.Current(StatId.ProjectileCapacity);
            var max = _character.Stat.Value(StatId.ProjectileCapacity);
            View.SetProjectileCapacity(cur, max);
        }

        private void OnBattleTag(BattleTag tag)
        {
            if (tag == BattleTags.Idle)
            {
                if (!_character.CanActivateAbility(BattleAbilityIds.Reload)) return;

                var reloadSpeed = _character.Stat.RateValue(StatId.ReloadSpeed);
                View.Reload(reloadSpeed);
            }

            if (tag == BattleTags.Aiming || tag == BattleTags.Firing)
            {
                View.CancelReload();
            }

            if (tag == BattleTags.Avoid)
            {
                if (!_character.CanActivateAbility(BattleAbilityIds.Avoid)) return;
                
                View.SetAvoidButton(Constant.AvoidCooldown);
            }
        }
    }
}