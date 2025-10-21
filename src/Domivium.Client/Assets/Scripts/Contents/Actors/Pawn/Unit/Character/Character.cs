using R3;
using UnityEngine;

namespace Domivium.Client.Contents.Actors
{
    public class Character : Unit
    {
        [SerializeField] private ParticleSystem _particleSystem;

        private DisposableBag _disposableBag;
        private readonly ReactiveProperty<bool> _isMove = new();

        protected override void OnAwake()
        {
            _isMove.Subscribe(Test).AddTo(ref _disposableBag);
            base.OnAwake();
        }

        private void Test(bool b)
        {
            if (b)
            {
                _particleSystem.Play();
            }
            else
            {
                _particleSystem.Stop();
            }
        }

        public override void Tick(float deltaTime)
        {
            _isMove.Value = _agent.velocity.sqrMagnitude > 0;

            base.Tick(deltaTime);
        }

        public bool IsRemainingDistance()
        {
            if (_agent.pathPending) return false;

            return _agent.remainingDistance > _agent.stoppingDistance;
        }
    }
}