using Domivium.Client.Core.Component.Text;
using UnityEngine;

namespace Domivium.Client.Core.Component.Stat
{
    public class FloatStat : MonoBehaviour
    {
        [SerializeField] private FloatText _valueText;

        public void Set(float value)
        {
            _valueText.Value = value;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}