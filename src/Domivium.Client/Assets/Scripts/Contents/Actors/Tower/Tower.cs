using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Actors.Contract;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Actors.Contract;
using UnityEngine;

namespace Domivium.Client.Contents.Actors
{
    public class Tower : Actor
    {
        public override UniTask ShowAsync(CancellationToken token, ActorParam param, bool immediately = false)
        {
            var p = param.As<TowerParams>();
            transform.localPosition = new Vector3(p.StartCell.x + 0.5f, 0, p.StartCell.y);
            return base.ShowAsync(token, param, immediately);
        }
    }
}