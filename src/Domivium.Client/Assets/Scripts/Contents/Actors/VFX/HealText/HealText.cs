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
    public class HealText : VFX
    {
        private const float StartX = -1.17f;
        private const float SizeX = 0.13f;

        private const float MoveUp = 0.8f;
        private const float MoveDown = 0.4f;

        [SerializeField] private Transform _driver;
        [SerializeField] private Transform _axis;
        [SerializeField] private SpriteNumber[] _numbers;
        [SerializeField] private Sprite[] _sprites;

        private readonly Queue<int> _cached = new();

        private Sequence _sequence;

        private Vector3 _basePos;
        private Vector3 _currentPosition;

        public override void Initialize(ushort uid, ActorId actorId, Transform parent)
        {
            base.Initialize(uid, actorId, parent);

            Reset();

            _sequence = DOTween.Sequence()
                .SetAutoKill(false)
                .Pause()
                .Append(_driver.DOLocalMoveY(MoveUp, 0.3f).SetEase(Ease.OutCirc).SetRecyclable(true))
                .Append(_driver.DOLocalMoveY(MoveDown, 0.9f).SetEase(Ease.Linear).SetRecyclable(true))
                .OnUpdate(() =>
                {
                    _currentPosition = _basePos + _driver.localPosition;
                    transform.localPosition = _currentPosition;
                })
                .SetLink(gameObject);
        }

        public override async UniTask SpawnAsync(CancellationToken token, ActorParam param)
        {
            await base.SpawnAsync(token, param);

            var p = param.As<HealTextParams>();
            var position = p.Position;
            var random = MathUtils.Random(0, 0.3f);
            position.x += random.x * 0.3f;
            position.y += 0.5f + random.y * 0.1f;

            _basePos = position;
            transform.localPosition = position;
            _axis.localPosition = Vector3.zero;
            _driver.localPosition = Vector3.zero;

            SetText(p.Heal);

            _sequence.Rewind();
            _sequence.Play();
        }

        public override void Despawn()
        {
            _sequence?.Pause();
            base.Despawn();
        }

        protected override void OnDestroyInternal()
        {
            base.OnDestroyInternal();
            _sequence?.Kill();
        }

        private void SetText(int heal)
        {
            if (heal <= 0) return;

            _cached.Clear();
            var cur = heal;
            while (cur > 10)
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
                _numbers[index].Show(_sprites[num], Color.green);
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