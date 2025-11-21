using Domivium.Client.Core.Component.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Domivium.Client.Core.Component.Stat
{
    public class GaugeStat : MonoBehaviour
    {
        [SerializeField] private IntLimitText _valueText;
        [SerializeField] private Image _gaugeImage;

        public void Set(int value, int limit)
        {
            _valueText.Value = value;
            _valueText.Limit = limit;
            _gaugeImage.fillAmount = (float)value / limit;
        }
    }
}