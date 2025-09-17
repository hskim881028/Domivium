using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Core.Actors.Contract;
using Domivium.Client.Core.Message;
using MessagePipe;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Domivium.Client.Core.Actors
{
    public abstract class ActorPresenter<TActor> : Disposable, IActorPresenter where TActor : Actor
    {
        protected Guid Id { get; }
        protected TActor Actor { get; }
        protected TagSet TagSet { get; } = new();
        protected readonly IPublisher<ActorTagMessage> TagPublisher;

        protected ActorPresenter(Guid id, TActor actor, IPublisher<ActorTagMessage> tagPublisher)
        {
            Id = id;
            Actor = actor;
            TagPublisher = tagPublisher;

            TagSet.Subscribe(OnChangedTags);
            TagSet.State.Subscribe(OnChangedStateTag).AddTo(ref DisposableBag);
        }

        public virtual void Initialize(Transform parent)
        {
            Actor.Initialize(parent);
        }

        public virtual async UniTask ActivateAsync(CancellationToken token, ActorParam param)
        {
            TagSet.Clear();
            await Actor.ActivateAsync(token, param);
        }

        public virtual void Deactivate()
        {
            TagSet.Clear();
            Actor.Deactivate();
        }

        protected override void OnDispose()
        {
            TagSet.Unsubscribe(OnChangedTags);
            base.OnDispose();
        }

        protected virtual void OnChangedStateTag(ActorTag tag)
        {
            TagPublisher.Publish(ActorTagMessage.Create(tag, Id));
        }

        protected virtual void OnChangedTags(in NotifyCollectionChangedEventArgs<ActorTag> e) { }
    }
}