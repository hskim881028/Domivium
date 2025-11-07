using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Core.Actors.Contract;
using UnityEngine;

namespace Domivium.Client.Core.Actors
{
    public interface IActorActivatable
    {
        public void Initialize(ushort uid, ActorId actorId, Transform parent);
        public UniTask SpawnAsync(CancellationToken token, ActorParam param);
        public void Activate();
        public void Despawn();
    }
}