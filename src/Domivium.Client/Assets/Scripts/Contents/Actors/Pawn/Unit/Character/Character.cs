using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Actors.Contract;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Actors.Contract;
using Domivium.Client.Core.Utility;
using R3;
using UnityEngine;

namespace Domivium.Client.Contents.Actors
{
    public class Character : Unit
    {
        [SerializeField] private ParticleSystem _particleSystem;
        [SerializeField] private LineRenderer _lineRenderer;

        private int _layerMask;

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
            _layerMask = Id == ActorId.Character ? Layer.MonsterOrPropMask : Layer.CharacterOrPropMask;
        }

        public void SetAim(Vector2 direction, float range)
        {
            var hit = Physics2D.Raycast(Muzzle.position, direction, range, _layerMask);
            if (hit.collider != null)
            {
                range = Vector2.Distance(Muzzle.position, hit.point);
            }

            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 180f;
            var rotation = Quaternion.Euler(0f, 0f, angle);
            Muzzle.rotation = rotation;

            _lineRenderer.SetPosition(0, Vector3.zero);
            _lineRenderer.SetPosition(1, Vector3.left * range);
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