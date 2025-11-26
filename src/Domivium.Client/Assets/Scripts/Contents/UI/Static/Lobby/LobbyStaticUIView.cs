using Domivium.Client.Contents.UIComponents;
using Domivium.Client.Core.UI.View;
using UnityEngine;

namespace Domivium.Client.Contents.UI.Static
{
    public class LobbyStaticUIView : StaticUIView<ILobbyStaticUIMessage>
    {
        [SerializeField] private BattleHud _battleHud;

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

        public void SetProjectileCapacity(int cur, int max)
        {
            _battleHud.SetProjectileCapacity(cur, max);
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