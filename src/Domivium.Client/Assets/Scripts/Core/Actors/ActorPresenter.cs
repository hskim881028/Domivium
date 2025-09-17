using System;
using System.Collections.Specialized;
using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Core.Actors.Contract;
using Domivium.Client.Core.Message;
using MessagePipe;
using ObservableCollections;
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
        }

        public virtual void Initialize(Transform parent)
        {
            Actor.Initialize(parent);
        }

        public virtual async UniTask ActivateAsync(CancellationToken token, ActorParam param)
        {
            TagSet.Clear();
            TagSet.Subscribe(OnChangedTags);
            await Actor.ActivateAsync(token, param);
        }

        public virtual void Deactivate()
        {
            TagSet.Unsubscribe(OnChangedTags);
            TagSet.Clear();
            Actor.Deactivate();
        }

        protected virtual void OnChangedTags(in NotifyCollectionChangedEventArgs<ActorTag> e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItem == ActorTag.Die)
                    {
                        TagPublisher.Publish(ActorTagMessage.Create(ActorTag.Die, Id));
                    }
                    else if (e.NewItem == ActorTag.Despawn)
                    {
                        TagPublisher.Publish(ActorTagMessage.Create(ActorTag.Despawn, Id));
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}