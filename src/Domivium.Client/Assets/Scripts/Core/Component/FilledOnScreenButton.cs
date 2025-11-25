using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.UI;

namespace Domivium.Client.Core.Component
{
    public class FilledOnScreenButton : OnScreenButton
    {
        [SerializeField] private Image _iconImage;
        [SerializeField] private Image _cooldownImage;
        [SerializeField] private Color _activeColor = Color.white;
        [SerializeField] private Color _disabledColor = Color.gray1;

        private readonly CancellationTokenSource _cts = new();

        public async UniTaskVoid SetAsync(float cooldown)
        {
            _iconImage.color = _disabledColor;
            _cooldownImage.color = _disabledColor;
            _cooldownImage.fillAmount = 0;

            var time = 0f;
            while (time < cooldown)
            {
                if (_cts.Token.IsCancellationRequested) break;

                time += Time.deltaTime;

                _cooldownImage.fillAmount = time / cooldown;
                await Awaitable.NextFrameAsync(_cts.Token);
            }

            _iconImage.color = _activeColor;
            _cooldownImage.color = _activeColor;
            _cooldownImage.fillAmount = 1;
        }

        private void Reset()
        {
            _cts.Cancel();
            _cts.Dispose();
        }
    }
}