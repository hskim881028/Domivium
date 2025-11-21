using Domivium.Client.Core.Utility;
using TMPro;
using UnityEngine;

namespace Domivium.Client.Core.Component.Text
{
    public class IntLimitText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private char[] _suffix;

        private readonly char[] _buf = new char[32];
        private int _value;
        private int _limit;

        public int Value
        {
            get => _value;
            set
            {
                _value = value;
                Refresh();
            }
        }

        public int Limit
        {
            get => _limit;
            set
            {
                _limit = value;
                Refresh();
            }
        }

        private void Refresh()
        {
            var length = TextWriteUtils.WriteIntGrouped(_value, _buf, ',');
            _buf[length++] = '/';
            length += TextWriteUtils.WriteIntGrouped(_limit, _buf, length, ',');

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