using Domivium.Client.Core.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Domivium.Client.Contents.UIComponents
{
    public class StatGauge : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Image _gaugeImage;

        private readonly char[] _buf = new char[32];

        public void Set(int cur, int max)
        {
            _gaugeImage.fillAmount = (float)cur / max;

            var length = TextWriteUtils.WriteIntGrouped(cur, _buf, ',');
            _buf[length++] = '/';
            length += TextWriteUtils.WriteIntGrouped(max, _buf, length, ',');
            _text.SetCharArray(_buf, 0, length);
        }
    }
}