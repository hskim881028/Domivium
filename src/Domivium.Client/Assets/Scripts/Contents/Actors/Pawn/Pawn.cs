using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Component;
using Domivium.Client.Data.Stat;
using UnityEngine;

namespace Domivium.Client.Contents.Actors
{
    public abstract class Pawn : Actor
    {
        [SerializeField] private HealthDisplay _healthDisplay;
        [SerializeField] private SpriteRenderer[] _renderers;
        [SerializeField] private Transform _muzzle;

        [SerializeField] private Collider2D _hitCollider;
        [SerializeField] private Collider2D _moveMoveCollider;

        public Collider2D MoveCollider => _moveMoveCollider;
        public Transform Muzzle => _muzzle;

        protected override void OnAwake()
        {
            base.OnAwake();
            if (_muzzle == null)
            {
                _muzzle = transform;
            }
        }

        public virtual void Die() { }

        public void SetFlip(bool value)
        {
            foreach (var r in _renderers)
            {
                r.flipX = value;
            }
        }

        public void SetHealth(int current, int max) => _healthDisplay.Set(current, max);

        public void SetRange(StatId statId, float range, Color color) { }
    }
}