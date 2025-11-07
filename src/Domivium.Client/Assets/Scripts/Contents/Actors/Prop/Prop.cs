using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Actors.Contract;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Actors.Contract;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Domivium.Client.Contents.Actors
{
    public class Prop : Actor
    {
        // [SerializeField] private List<ShadowCaster2D> _shadowCaster2D;

        public override async UniTask SpawnAsync(CancellationToken token, ActorParam param)
        {
            await base.SpawnAsync(token, param);
            var p = param.As<PropParams>();
            transform.localPosition = p.SpawnPosition;

            // foreach (var caster in _shadowCaster2D)
            // {
            //     caster.trimEdge = 0.1f;
            // }
        }
    }
}