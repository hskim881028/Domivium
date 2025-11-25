using Domivium.Client.Core.Utility;
using TMPro;
using UnityEngine;

namespace Domivium.Client.Core.Component.Text
{
    public class StringText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;

        private readonly char[] _buf = new char[32];
        private string _value;

        public string Value
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
            var length = TextWriteUtils.WriteString(_value, _buf);
            _text.SetCharArray(_buf, 0, length);
        }
    }
}