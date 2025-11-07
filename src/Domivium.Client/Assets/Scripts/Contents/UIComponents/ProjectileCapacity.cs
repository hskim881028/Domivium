using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Core.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Domivium.Client.Contents.UIComponents
{
    public class ProjectileCapacity : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Image _gaugeImage;

        private readonly char[] _buf = new char[32];
        private CancellationTokenSource _cts = new();

        public void CancelReload()
        {
            Reset();
        }

        public async UniTaskVoid Reload(float duration)
        {
            Reset();

            _cts = new CancellationTokenSource();
            var time = 0f;
            while (time < duration)
            {
                if (_cts.Token.IsCancellationRequested) break;

                time += Time.deltaTime;

                _gaugeImage.fillAmount = time / duration;
                await Awaitable.NextFrameAsync(_cts.Token);
            }

            _gaugeImage.fillAmount = 0;
        }

        public void Set(int cur, int max)
        {
            var length = TextWriteUtils.WriteIntGrouped(cur, _buf, ',');
            _buf[length++] = '/';
            length += TextWriteUtils.WriteIntGrouped(max, _buf, length, ',');
            _text.SetCharArray(_buf, 0, length);
        }

        private void Reset()
        {
            _gaugeImage.fillAmount = 0;

            if (_cts == null) return;

            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
        }
    }
}