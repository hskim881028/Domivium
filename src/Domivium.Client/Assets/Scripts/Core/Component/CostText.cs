using Domivium.Client.Core.Utility;
using TMPro;
using UnityEngine;

namespace Domivium.Client.Core.Component
{
    public class CostText : MonoBehaviour
    {
        private TextMeshProUGUI _text;
        private readonly char[] _buf = new char[16];
        private int _cost;

        public int Cost
        {
            get => _cost;
            set
            {
                _cost = value;
                var length = TextWriteUtils.WriteIntToBuffer(_cost, _buf);
                _text.SetCharArray(_buf, 0, length);
            }
        }

        private void Awake()
        {
            _text = GetComponentInChildren<TextMeshProUGUI>();
        }
    }
}