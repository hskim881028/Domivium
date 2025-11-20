using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Core.Actors.Contract;
using UnityEngine;

namespace Domivium.Client.Core.Actors
{
    public class Actor : MonoBehaviour, IActorActivatable, ITicker
    {
        public ushort Uid { get; private set; }
        public ActorId Id { get; private set; }

        private void Awake()
        {
            OnAwake();
        }

        private void OnDestroy()
        {
            OnDestroyInternal();
        }

        public virtual void Initialize(ushort uid, ActorId actorId, Transform parent)
        {
            Uid = uid;
            Id = actorId;
            transform.SetParent(parent);
        }

        public virtual async UniTask SpawnAsync(CancellationToken token, ActorParam param)
        {
            await UniTask.CompletedTask;
        }

        public virtual void Activate() { }

        public virtual void Despawn() { }

        public virtual void Tick(float deltaTime) { }

        protected virtual void OnAwake() { }

        protected virtual void OnDestroyInternal() { }
    }
}