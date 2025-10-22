using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Domivium.Client.Core.Utility;
using TMPro;
using UnityEngine;

namespace Domivium.Client.Core.Component
{
    public sealed class HealthDisplay : MonoBehaviour
    {
        private static readonly int FillId = Shader.PropertyToID("_Fill");

        [SerializeField] private SpriteRenderer _renderer;
        [SerializeField] private TextMeshPro _text;

        private readonly char[] _buf = new char[32];
        private MaterialPropertyBlock _mpb;

        private Sequence _sequence;

        private CancellationTokenSource _cts = new();

        private void Awake()
        {
            _mpb = new MaterialPropertyBlock();
            var length = TextWriteUtils.WriteIntGrouped(int.MinValue, _buf, ',');
            _text.SetCharArray(_buf, 0, length);
            _text.ForceMeshUpdate();
            _text.SetCharArray(Array.Empty<char>(), 0, 0);
            _renderer.enabled = false;
        }

        private void OnDestroy()
        {
            _cts.Cancel();
            _cts.Dispose();
        }

        public void Set(int current, int max)
        {
            if (current == 0 || current == max) return;

            _renderer.enabled = true;
            var ratio = (float)current / max;
            _renderer.GetPropertyBlock(_mpb);
            _mpb.SetFloat(FillId, ratio);
            _renderer.SetPropertyBlock(_mpb);

            var length = TextWriteUtils.WriteIntGrouped(current, _buf, ',');
            _buf[length++] = '/';
            length += TextWriteUtils.WriteIntGrouped(max, _buf, length, ',');
            _text.SetCharArray(_buf, 0, length);

            _cts.Cancel();
            _cts.Dispose();
            _cts = new CancellationTokenSource();
            Hide().Forget();
        }

        private async UniTaskVoid Hide()
        {
            await Awaitable.WaitForSecondsAsync(2f, _cts.Token);
            _renderer.enabled = false;
        }
    }
}