using Domivium.Client.Core.Utility;
using TMPro;
using UnityEngine;

namespace Domivium.Client.Core.Component.Text
{
    public class IntText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private char[] _prefix;
        [SerializeField] private char[] _suffix;

        private readonly char[] _buf = new char[16];
        private int _value;

        public int Value
        {
            get => _value;
            set
            {
                _value = value;
                Refresh();
            }
        }

        private void Refresh()
        {
            var length = _prefix is { Length: > 0 }
                ? TextWriteUtils.WriteWithPrefix(_value, _prefix, _buf)
                : TextWriteUtils.WriteIntToBuffer(_value, _buf);

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