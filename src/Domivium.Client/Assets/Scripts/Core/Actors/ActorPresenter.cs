using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Core.Actors.Contract;
using UnityEngine;

namespace Domivium.Client.Core.Actors
{
    public abstract class ActorPresenter<TActor> : Disposable, IActorPresenter where TActor : Actor
    {
        private Action _onDespawn;

        protected TActor Actor { get; }

        protected ActorPresenter(TActor actor)
        {
            Actor = actor;
        }

        public virtual void Initialize(Transform parent, Action onDespawn)
        {
            _onDespawn = onDespawn;
            Actor.Initialize(parent, onDespawn);
        }

        public virtual async UniTask ActivateAsync(CancellationToken token, ActorParam param, bool immediately = false)
        {
            await Actor.ActivateAsync(token, param, immediately);
        }

        public virtual async UniTask DeactivateAsync(CancellationToken token, bool immediately = false)
        {
            await Actor.DeactivateAsync(token, immediately);
            _onDespawn?.Invoke();
        }
    }
}