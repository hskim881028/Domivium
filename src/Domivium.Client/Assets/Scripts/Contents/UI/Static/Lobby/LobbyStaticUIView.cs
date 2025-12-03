using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.UIComponents;
using Domivium.Client.Core.UI.View;
using UnityEngine;

namespace Domivium.Client.Contents.UI.Static
{
    public class LobbyStaticUIView : StaticUIView<ILobbyStaticUIMessage>
    {
        [SerializeField] private BattleHud _battleHud;

        public override async UniTask<bool> InitializeAsync(CancellationToken token)
        {
            if (!await base.InitializeAsync(token)) return false;

            SetAvoidButton(0);
            SetInteractButton(false);
            return true;
        }

        public void SetAttackButton(bool canAttack)
        {
            _battleHud.SetAttackButton(canAttack);
        }

        public void SetAvoidButton(float cooldown)
        {
            _battleHud.SetAvoidButton(cooldown);
        }

        public void SetInteractButton(bool value)
        {
            _battleHud.SetInteractButton(value);
        }

        public void SetRemainProjectile(int value)
        {
            _battleHud.SetRemainCount(value);
        }

        public void SetLoadedProjectile(int value)
        {
            _battleHud.SetLoadedCount(value);
        }

        public void Reload(float duration)
        {
            _battleHud.Reload(duration);
        }

        public void CancelReload()
        {
            _battleHud.CancelReload();
        }
    }
}