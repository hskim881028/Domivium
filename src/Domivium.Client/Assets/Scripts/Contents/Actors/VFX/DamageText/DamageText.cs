using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Domivium.Client.Contents.Actors.Contract;
using Domivium.Client.Core.Actors.Contract;
using Domivium.Client.Core.Utility;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Domivium.Client.Contents.Actors
{
    public class DamageText : VFX
    {
        private static readonly Vector3 Punch1 = new(0f, 0.8f, 0f);
        private static readonly Vector3 Punch2 = new(0f, 0.4f, 0f);
        private static readonly Vector3 Punch3 = new(0f, 0.2f, 0f);
        private static readonly Vector3 Shake = new(0.05f, 0f, 0f);

        [SerializeField] private Transform _driver;
        [SerializeField] private Transform _axis;
        [SerializeField] private TextMeshPro _text;

        private readonly char[] _buf = new char[16];

        private Sequence _punch;
        private Sequence _shake;

        private Color _baseColor;
        private Vector3 _basePos;
        private Vector3 _currentPosition;

        public override void Initialize(Transform parent)
        {
            SetText(int.MinValue);
            _text.ForceMeshUpdate();
            _text.SetCharArray(Array.Empty<char>(), 0, 0);

            _baseColor = _text.color;
            _punch = DOTween.Sequence()
                .SetAutoKill(false)
                .Pause()
                .Append(_driver.DOPunchPosition(Punch1, 0.3f, 1, 0.8f).SetRecyclable(true))
                .Append(_driver.DOPunchPosition(Punch2, 0.3f, 2, 0.8f).SetRecyclable(true))
                .Append(_driver.DOPunchPosition(Punch3, 0.4f, 3, 0.5f).SetRecyclable(true))
                .Join(_text.DOFade(0f, 0.5f).SetDelay(0.4f).SetRecyclable(true))
                .OnUpdate(() =>
                {
                    _currentPosition = _basePos + _driver.localPosition;
                    transform.localPosition = _currentPosition;
                });

            _shake = DOTween.Sequence()
                .SetAutoKill(false)
                .Pause()
                .Append(_axis.DOShakePosition(0.8f, Shake).SetRecyclable(true));

            base.Initialize(parent);
        }

        public override UniTask ActivateAsync(CancellationToken token, ActorParam param)
        {
            var p = param.As<DamageTextParams>();
            var position = p.Position;
            var circle = Random.insideUnitCircle;
            position.x += circle.x * 0.3f;
            position.y += 0.5f + circle.y * 0.1f;

            _basePos = position;
            transform.localPosition = position;
            _axis.localPosition = Vector3.zero;
            _driver.localPosition = Vector3.zero;

            _text.color = _baseColor;
            SetText(p.Damage);

            _punch.Rewind();
            _punch.Play();

            _shake.Rewind();
            _shake.Play();
            return base.ActivateAsync(token, param);
        }

        public override void Deactivate()
        {
            _punch?.Pause();
            _shake?.Pause();

            _text.color = Color.clear;
            _text.SetCharArray(Array.Empty<char>(), 0, 0);
            base.Deactivate();
        }

        private void SetText(int damage)
        {
            var length = TextWriteUtility.WriteIntToBuffer(damage, _buf);
            _text.SetCharArray(_buf, 0, length);
        }
    }
}