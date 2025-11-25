using Domivium.Client.Core.Utility;
using TMPro;
using UnityEngine;

namespace Domivium.Client.Core.Component.Text
{
    public class FloatLimitText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private char[] _suffix;

        private readonly char[] _buf = new char[32];
        private float _value;
        private float _limit;

        public float Value
        {
            get => _value;
            set
            {
                _value = value;
                Refresh();
            }
        }

        public float Limit
        {
            get => _limit;
            set
            {
                _limit = value;
                Refresh();
            }
        }

        public Color Color
        {
            get => _text.color;
            set => _text.color = value;
        }

        private void Refresh()
        {
            var length = TextWriteUtils.WriteIntGroupedFixed2(_value, _buf, ',');
            _buf[length++] = '/';
            length += TextWriteUtils.WriteIntGroupedFixed2(_limit, _buf, length, ',');

            if (_suffix is { Length: > 0 })
            {
                foreach (var c in _suffix)
                {
                    _buf[length++] = c;
                }
            }

            _text.SetCharArray(_buf, 0, length);
        }
    }
}