using Domivium.Client.Contents.UIComponents;
using Domivium.Client.Core.UI.View;
using Domivium.Client.Data.Stat;
using UnityEngine;

namespace Domivium.Client.Contents.UI.Static
{
    public class StageStaticUIView : StaticUIView<IStageStaticUIMessage>
    {
        [SerializeField] private BattleHud _battleHud;
        [SerializeField] private StatGaugeHud _statGaugeHud;

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

        public void SetStatGauge(StatId statId, int current, int limit)
        {
            _statGaugeHud.Set(statId, current, limit);
        }
    }
}