using Domivium.Client.Core.Component.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Domivium.Client.Core.Component.Stat
{
    public class SliderStat : MonoBehaviour
    {
        [SerializeField] private IntLimitText _valueText;
        [SerializeField] private Slider _slider;

        public void Set(int current, int limit)
        {
            _valueText.Value = current;
            _valueText.Limit = limit;
            _slider.maxValue = limit;
            _slider.value = current;
        }
    }
}