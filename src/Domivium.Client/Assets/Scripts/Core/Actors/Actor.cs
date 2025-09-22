using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Core.Actors.Contract;
using UnityEngine;

namespace Domivium.Client.Core.Actors
{
    public class Actor : MonoBehaviour, IActorActivatable, ITicker
    {
        public Guid Id { get; private set; }

        public virtual void Initialize(Guid id, Transform parent)
        {
            Id = id;
            transform.SetParent(parent);
        }

        public virtual UniTask ActivateAsync(CancellationToken token, ActorParam param) => UniTask.CompletedTask;

        public virtual void Deactivate() { }

        public virtual void Tick(float deltaTime) { }

        protected virtual void OnDestroyInternal() { }

        private void OnDestroy()
        {
            OnDestroyInternal();
        }
    }
}