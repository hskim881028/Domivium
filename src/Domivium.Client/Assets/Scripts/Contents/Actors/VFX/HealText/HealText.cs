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
    public class HealText : VFX
    {
        private static readonly int ColorId = Shader.PropertyToID("_FaceColor");
        private static readonly int OutlineId = Shader.PropertyToID("_OutlineWidth");

        private const float MoveUp = 0.8f;
        private const float MoveDown = 0.4f;

        [SerializeField] private Transform _driver;
        [SerializeField] private Transform _axis;
        [SerializeField] private TextMeshPro _text;
        [SerializeField] private MeshRenderer _renderer;

        private readonly char[] _buf = new char[16];

        private Sequence _sequence;

        private MaterialPropertyBlock _materialPropertyBlock;
        private Vector3 _basePos;
        private Vector3 _currentPosition;


        public override void Initialize(ushort id, Transform parent)
        {
            SetText(int.MinValue);
            _text.ForceMeshUpdate();
            _text.SetCharArray(Array.Empty<char>(), 0, 0);

            var mpb = new MaterialPropertyBlock();
            _renderer.GetPropertyBlock(mpb);
            _materialPropertyBlock = mpb;
            _materialPropertyBlock.SetColor(ColorId, Color.greenYellow);
            _materialPropertyBlock.SetFloat(OutlineId, 0.3f);
            _renderer.SetPropertyBlock(_materialPropertyBlock);
            _sequence = DOTween.Sequence()
                .SetAutoKill(false)
                .Pause()
                .Append(_driver.DOLocalMoveY(MoveUp, 0.3f).SetEase(Ease.OutCirc).SetRecyclable(true))
                .Append(_driver.DOLocalMoveY(MoveDown, 0.9f).SetEase(Ease.Linear).SetRecyclable(true))
                .Join(_text.DOFade(0f, 0.5f).SetDelay(0.4f).SetRecyclable(true))
                .OnUpdate(() =>
                {
                    _currentPosition = _basePos + _driver.localPosition;
                    transform.localPosition = _currentPosition;
                });

            base.Initialize(id, parent);
        }

        public override UniTask ActivateAsync(CancellationToken token, ActorParam param)
        {
            var p = param.As<HealTextParams>();
            var position = p.Position;
            var circle = Random.insideUnitCircle;
            position.x += circle.x * 0.3f;
            position.y += 0.5f + circle.y * 0.1f;

            _basePos = position;
            transform.localPosition = position;
            _axis.localPosition = Vector3.zero;
            _driver.localPosition = Vector3.zero;

            _text.color = Color.white;
            SetText(p.Damage);

            _sequence.Rewind();
            _sequence.Play();

            return base.ActivateAsync(token, param);
        }

        public override void Deactivate()
        {
            _sequence?.Pause();

            _text.color = Color.clear;
            _text.SetCharArray(Array.Empty<char>(), 0, 0);
            base.Deactivate();
        }

        protected override void OnDestroyInternal()
        {
            _sequence?.Kill();
            base.OnDestroyInternal();
        }

        private void SetText(int damage)
        {
            var length = TextWriteUtils.WriteIntToBuffer(damage, _buf);
            _text.SetCharArray(_buf, 0, length);
        }
    }
}