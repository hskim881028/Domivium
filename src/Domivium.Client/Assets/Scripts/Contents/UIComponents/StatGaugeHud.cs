using Domivium.Client.Core.Component.Stat;
using Domivium.Client.Data.Stat;
using UnityEngine;

namespace Domivium.Client.Contents.UIComponents
{
    public class StatGaugeHud : MonoBehaviour
    {
        [SerializeField] private GaugeStat _health;
        [SerializeField] private GaugeStat _hunger;
        [SerializeField] private GaugeStat _stamina;
        [SerializeField] private GaugeStat _sanity;

        public void Set(StatId statId, int current, int limit)
        {
            if (statId == StatId.Health)
            {
                _health.Set(current, limit);
            }
            else if (statId == StatId.Hunger)
            {
                _hunger.Set(current, limit);
            }
            else if (statId == StatId.Stamina)
            {
                _stamina.Set(current, limit);
            }
            else if (statId == StatId.Sanity)
            {
                _sanity.Set(current, limit);
            }
        }
    }
}