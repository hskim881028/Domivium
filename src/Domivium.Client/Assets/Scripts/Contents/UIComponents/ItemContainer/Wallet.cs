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

        public void SetWeight(float current, float limit)
        {
            _weightImage.color = current < limit ? Color.white : Color.red;
            _weightText.Color = current < limit ? Color.white : Color.red;
            _weightText.Value = current * Constant.Percent;
            _weightText.Limit = limit * Constant.Percent;
            _weightSlider.maxValue = limit;
            _weightSlider.value = Mathf.Min(current, limit);
        }

        public void SetMoney(int value)
        {
            _moneyText.Value = value;
        }

        public void SetGem(int value)
        {
            _gemText.Value = value;
        }
    }
}