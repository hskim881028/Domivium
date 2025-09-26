using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Core.Actors.Contract;
using UnityEngine;

namespace Domivium.Client.Core.Actors
{
    public class Actor : MonoBehaviour, IActorActivatable, ITicker
    {
        public ushort Id { get; private set; }

        private void Awake()
        {
            OnAwake();
        }

        private void OnDestroy()
        {
            OnDestroyInternal();
        }

        public virtual void Initialize(ushort id, Transform parent)
        {
            Id = id;
            transform.SetParent(parent);
        }

        public virtual UniTask ActivateAsync(CancellationToken token, ActorParam param) => UniTask.CompletedTask;

        public virtual void Deactivate() { }

        public virtual void Tick(float deltaTime) { }

        protected virtual void OnAwake() { }
        protected virtual void OnDestroyInternal() { }
    }
}