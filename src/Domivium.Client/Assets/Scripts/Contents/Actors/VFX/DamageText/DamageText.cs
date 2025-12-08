using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Domivium.Client.Contents.Actors.Contract;
using Domivium.Client.Contents.Components;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Actors.Contract;
using Domivium.Client.Core.Utility;
using UnityEngine;

namespace Domivium.Client.Contents.Actors
{
    public class DamageText : VFX
    {
        private const float StartX = -1.17f;
        private const float SizeX = 0.13f;
        private const float MoveY = 0.8f;

        private static readonly Vector3 Punch1 = new(0f, -0.2f, 0f);
        private static readonly Vector3 Punch2 = new(0f, -0.1f, 0f);
        private static readonly Vector3 Shake = new(0.05f, 0f, 0f);

        [SerializeField] private Transform _driver;
        [SerializeField] private Transform _axis;
        [SerializeField] private SpriteNumber[] _numbers;
        [SerializeField] private Sprite[] _sprites;

        private readonly Queue<int> _cached = new();

        private Sequence _punch;
        private Sequence _shake;

        private Vector3 _basePos;
        private Vector3 _currentPosition;

        public override void Initialize(ushort uid, ActorId actorId, Transform parent)
        {
            base.Initialize(uid, actorId, parent);

            Reset();

            _punch = DOTween.Sequence()
                .SetAutoKill(false)
                .Pause()
                .Append(_driver.DOLocalMoveY(MoveY, 0.3f).SetEase(Ease.OutCirc).SetRecyclable(true))
                .Append(_driver.DOPunchPosition(Punch1, 0.3f, 2, 0.8f).SetRecyclable(true))
                .Append(_driver.DOPunchPosition(Punch2, 0.4f, 3, 0.5f).SetRecyclable(true))
                .OnUpdate(() =>
                {
                    _currentPosition = _basePos + _driver.localPosition;
                    transform.localPosition = _currentPosition;
                })
                .SetLink(gameObject);

            _shake = DOTween.Sequence()
                .SetAutoKill(false)
                .Pause()
                .Append(_axis.DOShakePosition(0.8f, Shake).SetDelay(0.3f).SetRecyclable(true))
                .SetLink(gameObject);
        }

        public override async UniTask SpawnAsync(CancellationToken token, ActorParam param)
        {
            await base.SpawnAsync(token, param);

            var p = param.As<DamageTextParams>();
            var position = p.Position;
            var random = MathUtils.Random(0, 0.3f);
            position.x += random.x;
            position.y += 0.5f + random.y;

            _basePos = position;
            transform.localPosition = position;
            _axis.localPosition = Vector3.zero;
            _driver.localPosition = Vector3.zero;

            SetText(p.Damage);

            _punch.Rewind();
            _punch.Play();

            _shake.Rewind();
            _shake.Play();
        }

        public override void Despawn()
        {
            _punch?.Pause();
            _shake?.Pause();
            base.Despawn();
        }

        protected override void OnDestroyInternal()
        {
            base.OnDestroyInternal();
            _punch?.Kill();
            _shake?.Kill();
        }

        private void SetText(int damage)
        {
            if (damage <= 0) return;

            _cached.Clear();
            var cur = damage;
            while (cur >= 10)
            {
                var d = cur % 10;
                _cached.Enqueue(d);
                cur /= 10;
            }

            _cached.Enqueue(cur);

            Reset();

            var index = 0;
            while (_cached.Count > 0)
            {
                var num = _cached.Dequeue();
                _numbers[index].Show(_sprites[num], Color.red);
                index++;
            }

            _axis.localPosition = new Vector3(StartX + (index - 1) * SizeX, 0, 0);
        }

        private void Reset()
        {
            foreach (var number in _numbers)
            {
                number.Hide();
            }
        }
    }
}