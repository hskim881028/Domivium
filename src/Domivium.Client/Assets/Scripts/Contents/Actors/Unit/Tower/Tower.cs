using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Actors.Contract;
using Domivium.Client.Core.Actors.Contract;
using UnityEngine;

namespace Domivium.Client.Contents.Actors
{
    public class Tower : Unit
    {
        public override UniTask ActivateAsync(CancellationToken token, ActorParam param)
        {
            var p = param.As<TowerParams>();
            transform.localPosition = new Vector3(p.StartCell.x + 0.5f, 0, p.StartCell.y);
            return base.ActivateAsync(token, param);
        }
    }
}