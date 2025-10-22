using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Domivium.Client.Contents.Actors.Contract;
using Domivium.Client.Core.Actors.Contract;
using UnityEngine;

namespace Domivium.Client.Contents.Actors
{
    public class SoulEffect : VFX
    {
        [SerializeField] private ParticleSystem _particleSystem;
        [SerializeField] private TrailRenderer _trailRenderer;

        protected override void OnAwake()
        {
            _particleSystem.Stop();
            base.OnAwake();
        }

        public override UniTask ActivateAsync(CancellationToken token, ActorParam param)
        {
            var p = param.As<SoulEffectParams>();
            var position = p.Position;
            var circle = Random.insideUnitCircle * 0.1f;
            transform.localPosition = new Vector3(position.x + circle.x, 0, position.z + circle.y);

            _trailRenderer.Clear();
            _particleSystem.Play();

            var endPosition = new Vector3(p.EndPosition.x, 0, p.EndPosition.z + 0.5f);
            transform.DOMove(endPosition, 1).SetDelay(1.5f).SetEase(Ease.OutCirc);
            return base.ActivateAsync(token, param);
        }

        public override void Deactivate()
        {
            _trailRenderer.Clear();
            _particleSystem.Stop();
            base.Deactivate();
        }
    }
}