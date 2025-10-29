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

        private readonly Dictionary<StatId, DebugCircle> _ranges = new();

        public override UniTask ActivateAsync(CancellationToken token, ActorParam param)
        {
            var p = param.As<UnitParams>();
            transform.localPosition = new Vector3(p.SpawnPoint.x + 0.5f, 0, p.SpawnPoint.y);
            return base.ActivateAsync(token, param);
        }

        public virtual void Die() { }

        public virtual void StartBattle(Vector3 offset, Vector3 target) { }

        public virtual void Battle(Vector3 target) { }

        public void SetFlip(bool isRight)
        {
            _renderer.flipX = isRight;
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