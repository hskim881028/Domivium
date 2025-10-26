using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Actors.Contract;
using Domivium.Client.Core.Actors.Contract;
using R3;
using UnityEngine;

namespace Domivium.Client.Contents.Actors
{
    public class Character : Unit
    {
        private static readonly int RimColorId = Shader.PropertyToID("_RimColor");

        [SerializeField] private ParticleSystem _particleSystem;

        private DisposableBag _disposableBag;
        private readonly ReactiveProperty<bool> _isMove = new();

        protected override void OnAwake()
        {
            _isMove.Subscribe(Test).AddTo(ref _disposableBag);
            base.OnAwake();
        }

        public override UniTask ActivateAsync(CancellationToken token, ActorParam param)
        {
            var p = param.As<UnitParams>();
            var color = p.PawnContext.PawnType.ToColor();
            MaterialPropertyBlocks[0].SetColor(RimColorId, color);
            _renderer[0].SetPropertyBlock(MaterialPropertyBlocks[0]);
            return base.ActivateAsync(token, param);
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