using Domivium.Client.Core.Component.Text;
using UnityEngine;

namespace Domivium.Client.Core.Component.Stat
{
    public class LimitFloatStat : MonoBehaviour
    {
        [SerializeField] private FloatLimitText _valueText;

        public void Set(float current, float limit)
        {
            _valueText.Value = current;
            _valueText.Limit = limit;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}