using Domivium.Client.Core.Component.Text;
using UnityEngine;

namespace Domivium.Client.Core.Component.Stat
{
    public class IntStat : MonoBehaviour
    {
        [SerializeField] private IntText _valueText;

        public void Set(int value)
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