using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Actors.Contract;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Actors.Contract;

namespace Domivium.Client.Contents.Actors
{
    public class Prop : Actor
    {
        public override async UniTask SpawnAsync(CancellationToken token, ActorParam param)
        {
            await base.SpawnAsync(token, param);
            var p = param.As<PropParams>();
            transform.localPosition = p.SpawnPosition;
        }
    }
}