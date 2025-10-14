using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Actors.Contract;
using Domivium.Client.Core.Actors.Contract;
using UnityEngine;

namespace Domivium.Client.Contents.Actors
{
    public class Nexus : Pawn
    {
        public override UniTask ActivateAsync(CancellationToken token, ActorParam param)
        {
            var p = param.As<UnitParams>();
            transform.localPosition = new Vector3(p.SpawnPoint.x + 0.5f, 0, p.SpawnPoint.y);
            return base.ActivateAsync(token, param);
        }
    }
}