using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Domivium.Client.Contents.Actors
{
    public class Monster : Unit
    {
        private SpriteRenderer[] _spriteRenderers;
        private CancellationTokenSource _cts = new();

        protected override void OnAwake()
        {
            base.OnAwake();
            _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
            // foreach (var spriteRenderer in _spriteRenderers)
            // {
            //     spriteRenderer.maskInteraction = SpriteMaskInteraction.None;
            // }
        }

        public async UniTaskVoid ShowAsync(float duration = 1)
        {
            CancelAndDisposeCts();
            _cts = new CancellationTokenSource();

            foreach (var spriteRenderer in _spriteRenderers)
            {
                spriteRenderer.maskInteraction = SpriteMaskInteraction.None;
            }
            await Awaitable.WaitForSecondsAsync(duration, _cts.Token);
            foreach (var spriteRenderer in _spriteRenderers)
            {
                spriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            }
        }

        private void CancelAndDisposeCts()
        {
            if (_cts == null) return;

            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
        }
    }
}