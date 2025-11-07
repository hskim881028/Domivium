using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Core.Actors.Contract;
using UnityEngine;

namespace Domivium.Client.Contents.Actors
{
    public class Projectile : Pawn
    {
        [SerializeField] private TrailRenderer _trailRenderer;

        protected override void OnAwake()
        {
            base.OnAwake();
            _trailRenderer.enabled = true;
        }

        public override async UniTask SpawnAsync(CancellationToken token, ActorParam param)
        {
            await base.SpawnAsync(token, param);
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