using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Actors.Contract;
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
        [SerializeField] private Transform _lootAt;
        [SerializeField] private GameObject _aim;

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
            // var color = p.PawnContext.PawnType.ToColor();
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

        public void SetAim(Vector2 direction)
        {
            _aim.SetActive(!Mathf.Approximately(direction.sqrMagnitude, 0));

            var origin = _lootAt.transform.position;
            var dir = new Vector3(direction.x, direction.y, 0);
            var dist = 10f;
            var hit = Physics2D.Raycast(origin, dir, dist, Layer.PropMask);
            if (hit.collider != null)
            {
                dist = Vector2.Distance(_lootAt.position, hit.point);
            }

            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 180f;
            _lootAt.rotation = Quaternion.Euler(0f, 0f, angle);

            _lineRenderer.SetPosition(0, Vector3.zero);
            _lineRenderer.SetPosition(1, Vector3.left * dist);
        }
    }
}