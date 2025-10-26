using Domivium.Client.Core.Utility;
using TMPro;
using UnityEngine;

namespace Domivium.Client.Core.Component
{
    public class LevelDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshPro _text;

        private readonly char[] _buf = new char[16];

        public void Set(int level)
        {
            var length = TextWriteUtils.WriteIntToBuffer(level, _buf);
            _text.SetCharArray(_buf, 0, length);
        }
    }
}