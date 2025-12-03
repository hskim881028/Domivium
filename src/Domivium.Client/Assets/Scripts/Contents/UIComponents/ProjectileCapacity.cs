using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Core.Component.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Domivium.Client.Contents.UIComponents
{
    public class ProjectileCapacity : MonoBehaviour
    {
        [SerializeField] private IntLimitText _text;
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

        public void SetRemainCount(int value)
        {
            _text.Limit = value;
        }

        public void SetLoadedCount(int value)
        {
            _text.Value = value;
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