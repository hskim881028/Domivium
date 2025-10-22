using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Core.Actors.Contract;
using UnityEngine;

namespace Domivium.Client.Core.Actors
{
    public class Actor : MonoBehaviour, IActorActivatable, ITicker
    {
        public ushort Uid { get; private set; }

        private void Awake()
        {
            OnAwake();
        }

        private void Update()
        {
            OnUpdate();
        }

        private void OnDestroy()
        {
            OnDestroyInternal();
        }

        public virtual void Initialize(ushort uid, Transform parent)
        {
            Uid = uid;
            transform.SetParent(parent);
        }

        public virtual UniTask ActivateAsync(CancellationToken token, ActorParam param) => UniTask.CompletedTask;

        public virtual void Deactivate() { }

        public virtual void Tick(float deltaTime) { }

        protected virtual void OnAwake() { }
        protected virtual void OnUpdate() { }
        protected virtual void OnDestroyInternal() { }
    }
}