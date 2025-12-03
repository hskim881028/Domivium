using System.Collections.Generic;
using Domivium.Client.Contents.Battle;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Audio;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Container;
using Domivium.Client.Core.UI;
using Domivium.Client.Core.UI.Navigation;
using Domivium.Client.Core.UI.Presenter;
using Domivium.Client.Data.Stat;
using R3;
using UnityEngine;

namespace Domivium.Client.Contents.UI.Static
{
    public class LobbyStaticUIPresenter : StaticUIPresenter<LobbyStaticUIView, ILobbyStaticUIMessage>, ILobbyStaticUIMessage
    {
        private IBattleSystem _character;

        protected override HashSet<UILayer> Layer => UILayer.Set(UILayers.Lobby);
        public override UIPriority Priority => UIPriorities.Lobby;

        public LobbyStaticUIPresenter(
            LobbyStaticUIView view,
            IUINavigation navigation,
            IAudioPlayer audioPlayer,
            IActorManager actorManager,
            IUserContainer userContainer) : base(view, navigation, audioPlayer)
        {
            actorManager.Character.Subscribe(OnChangeCharacter).AddTo(ref DisposableBag);
            userContainer.LoadedProjectile.Subscribe(OnChangedLoadedProjectile).AddTo(ref DisposableBag);
            userContainer.RemainProjectile.Subscribe(OnChangedRemainProjectile).AddTo(ref DisposableBag);
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

        private void OnLookAt(Vector2 value)
        {
            View.SetAttackButton(value.sqrMagnitude > Constant.CanAttackRange);
        }

        private void OnChangedLoadedProjectile(int count)
        {
            View.SetLoadedProjectile(count);
        }

        private void OnChangedRemainProjectile(int count)
        {
            View.SetRemainProjectile(count);
        }

        private void OnChangeCharacter(IUnitPresenter character)
        {
            if (character == null) return;

            _character = character.BattleSystem;
            _character.OnActivateAbility.Subscribe(OnActivateAbility).AddTo(ref DisposableBag);
            _character.LookAt.Subscribe(OnLookAt).AddTo(ref DisposableBag);
        }
    }
}