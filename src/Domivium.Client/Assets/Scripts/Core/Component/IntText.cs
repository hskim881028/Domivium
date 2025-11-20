using Domivium.Client.Core.Utility;
using TMPro;
using UnityEngine;

namespace Domivium.Client.Core.Component
{
    public class IntText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;

        private readonly char[] _buf = new char[16];
        private int _value;

        public int Value
        {
            get => _value;
            set
            {
                _value = value;
                var length = TextWriteUtils.WriteIntToBuffer(_value, _buf);
                _text.SetCharArray(_buf, 0, length);
            }
        }
    }
}