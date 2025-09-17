using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Core.Actors.Contract;
using UnityEngine;

namespace Domivium.Client.Core.Actors
{
    public class Actor : MonoBehaviour, IActorActivatable
    {
        public virtual void Initialize(Transform parent)
        {
            transform.SetParent(parent);
        }

        public virtual UniTask ActivateAsync(CancellationToken token, ActorParam param) => UniTask.CompletedTask;

        public virtual void Deactivate() { }
    }
}