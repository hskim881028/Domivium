using Domivium.Client.Core.Component.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Domivium.Client.Core.Component.Stat
{
    public class SliderStat : MonoBehaviour
    {
        [SerializeField] private IntLimitText _valueText;
        [SerializeField] private Slider _slider;

        public void Set(int value, int limit)
        {
            _valueText.Value = value;
            _valueText.Limit = limit;
            _slider.value = value;
            _slider.maxValue = limit;
        }
    }
}