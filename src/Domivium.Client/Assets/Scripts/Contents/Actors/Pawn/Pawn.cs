using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Actors.Contract;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Actors.Contract;
using Domivium.Client.Core.Component;
using Domivium.Client.Data.Stat;
using UnityEngine;

namespace Domivium.Client.Contents.Actors
{
    public abstract class Pawn : Actor
    {
        [SerializeField] private DebugCircle _debugCircle;
        [SerializeField] private HealthDisplay _healthDisplay;
        [SerializeField] private SpriteRenderer _renderer;
        [SerializeField] private Transform _muzzle;

        private Collider2D _collider;

        private readonly Dictionary<StatId, DebugCircle> _ranges = new();
        private bool _flipX;

        public Collider2D Collider => _collider;
        public Transform Muzzle => _muzzle;


        public override async UniTask SpawnAsync(CancellationToken token, ActorParam param)
        {
            await base.SpawnAsync(token, param);
            var p = param.As<PawnParams>();
            transform.localPosition = p.SpawnPosition;
        }

        protected override void OnAwake()
        {
            base.OnAwake();
            _collider = GetComponentInChildren<Collider2D>();
            if (_muzzle == null)
            {
                _muzzle = transform;
            }
        }

        public virtual void Die() { }

        public virtual void StartBattle(Vector3 offset, Vector3 target) { }

        public virtual void Battle(Vector3 target) { }

        public void SetFlip(float x)
        {
            if (Mathf.Approximately(x, 0f)) return;

            var flipX = x > 0;
            if (_flipX == flipX) return;

            _flipX = flipX;
            _renderer.flipX = _flipX;
        }

        public void SetHealth(int current, int max) => _healthDisplay.Set(current, max);

        public void SetRange(StatId statId, float range, Color color)
        {
            if (!_ranges.ContainsKey(statId))
            {
                var circle = Instantiate(_debugCircle, transform);
                circle.gameObject.layer = gameObject.layer;
                _ranges.Add(statId, circle);
            }

            _ranges[statId].Draw(range, color);
        }

        private void SetActiveIndicator(bool isActive)
        {
            _healthDisplay.gameObject.SetActive(isActive);
            foreach (var circle in _ranges.Values)
            {
                circle.gameObject.SetActive(isActive);
            }
        }
    }
}