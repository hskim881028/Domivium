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
        [SerializeField] private ParticleSystem _particleSystem;

        private DisposableBag _disposableBag;
        private readonly ReactiveProperty<bool> _isMove = new();

        protected override void OnAwake()
        {
            base.OnAwake();
            _isMove.Subscribe(Test).AddTo(ref _disposableBag);
        }

        public override async UniTask SpawnAsync(CancellationToken token, ActorParam param)
        {
            await base.SpawnAsync(token, param);
            var p = param.As<CharacterParams>();
            // var color = p.PawnContext.PawnType.ToColor();
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
    }
}