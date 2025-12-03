using Domivium.Client.Core.Component.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Domivium.Client.Contents.UIComponents.ItemContainer
{
    public class Wallet : MonoBehaviour
    {
        [SerializeField] private Image _weightImage;
        [SerializeField] private FloatLimitText _weightText;
        [SerializeField] private Slider _weightSlider;
        [SerializeField] private IntText _moneyText;
        [SerializeField] private IntText _gemText;

        public void SetFilledWeightCapacity(float value)
        {
            _weightText.Value = value * Constant.Percent;
            if (_weightSlider.maxValue < value)
            {
                _weightSlider.maxValue = value;
            }

            _weightSlider.value = value;
            SetWeightColor();
        }

        public void SetWeightCapacity(float value)
        {
            _weightText.Limit = value * Constant.Percent;
            _weightSlider.maxValue = value;
            if (value < _weightSlider.value)
            {
                _weightSlider.value = value;
            }
            SetWeightColor();
        }

        public void SetMoney(int value)
        {
            _moneyText.Value = value;
        }

        public void SetGem(int value)
        {
            _gemText.Value = value;
        }

        private void SetWeightColor()
        {
            var color = _weightText.Value < _weightText.Limit ? Color.white : Color.red;
            _weightImage.color = color;
            _weightText.Color = color;
        }
    }
}