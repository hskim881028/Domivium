using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Core.Actors.Contract;
using UnityEngine;

namespace Domivium.Client.Core.Actors
{
    public class Actor : MonoBehaviour, IActorActivatable
    {
        public virtual void Initialize(Transform parent, Action onDespawn)
        {
            transform.SetParent(parent);
        }

        public virtual UniTask ShowAsync(CancellationToken token, ActorParam param, bool immediately = false) => UniTask.CompletedTask;

        public virtual UniTask HideAsync(CancellationToken token, bool immediately = false) => UniTask.CompletedTask;
    }
}