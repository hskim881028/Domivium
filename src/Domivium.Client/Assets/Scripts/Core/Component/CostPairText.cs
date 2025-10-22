using Domivium.Client.Core.Utility;
using TMPro;
using UnityEngine;

namespace Domivium.Client.Core.Component
{
    public class CostPairText : MonoBehaviour
    {
        private TextMeshProUGUI _text;
        private readonly char[] _buf = new char[32];
        private int _cost;
        private int _limit;

        public int Cost
        {
            get => _cost;
            set
            {
                _cost = value;
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

        private void Awake()
        {
            _text = GetComponentInChildren<TextMeshProUGUI>();
        }

        private void Set()
        {
            var length = TextWriteUtils.WriteIntGrouped(_cost, _buf, ',');
            _buf[length++] = '/';
            length += TextWriteUtils.WriteIntGrouped(_limit, _buf, length, ',');
            _text.SetCharArray(_buf, 0, length);
        }
    }
}