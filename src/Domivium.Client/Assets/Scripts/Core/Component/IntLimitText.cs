using Domivium.Client.Core.Utility;
using TMPro;
using UnityEngine;

namespace Domivium.Client.Core.Component
{
    public class IntLimitText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;

        private readonly char[] _buf = new char[32];
        private int _value;
        private int _limit;

        public int Value
        {
            get => _value;
            set
            {
                _value = value;
                Set();
            }
        }

        public int Limit
        {
            get => _limit;
            set
            {
                _limit = value;
                Set();
            }
        }

        private void Set()
        {
            var length = TextWriteUtils.WriteIntGrouped(_value, _buf, ',');
            _buf[length++] = '/';
            length += TextWriteUtils.WriteIntGrouped(_limit, _buf, length, ',');
            _text.SetCharArray(_buf, 0, length);
        }
    }
}