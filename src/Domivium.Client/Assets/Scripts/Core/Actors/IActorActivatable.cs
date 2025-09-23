using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Core.Actors.Contract;
using UnityEngine;

namespace Domivium.Client.Core.Actors
{
    public interface IActorActivatable
    {
        public void Initialize(ushort id, Transform parent);
        public UniTask ActivateAsync(CancellationToken token, ActorParam param);
        public void Deactivate();
    }
}