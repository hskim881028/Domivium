using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Actors.Contract;
using Domivium.Client.Core.Actors.Contract;
using UnityEngine;

namespace Domivium.Client.Contents.Actors
{
    public class Projectile : Pawn
    {
        [SerializeField] private TrailRenderer _trailRenderer;
        [SerializeField] private SpriteRenderer _body;

        protected override void OnAwake()
        {
            base.OnAwake();
            _trailRenderer.enabled = false;
        }

        public override async UniTask SpawnAsync(CancellationToken token, ActorParam param)
        {
            await base.SpawnAsync(token, param);
            var p = param.As<ProjectileParams>();
            _body.sprite = SpriteSystem.GetProjectile(p.SourceActorId, p.Id);

            _trailRenderer.Clear();
            _trailRenderer.enabled = true;
        }

        public override void Despawn()
        {
            _trailRenderer.Clear();
            _trailRenderer.enabled = false;
            base.Despawn();
        }
    }
}