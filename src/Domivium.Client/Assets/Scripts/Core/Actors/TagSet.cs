using System.Collections.Generic;
using ObservableCollections;

namespace Domivium.Client.Core.Actors
{
    public sealed class TagSet
    {
        private readonly ObservableHashSet<ActorTag> _tags = new();

        public IReadOnlyCollection<ActorTag> Tags => _tags;

        public void Subscribe(NotifyCollectionChangedEventHandler<ActorTag> handler) => _tags.CollectionChanged += handler;

        public void Unsubscribe(NotifyCollectionChangedEventHandler<ActorTag> handler) => _tags.CollectionChanged -= handler;
        public bool Contains(ActorTag tag) => _tags.Contains(tag);
        public void Add(ActorTag tag) => _tags.Add(tag);
        public void Remove(ActorTag tag) => _tags.Remove(tag);
        public void Clear() => _tags.Clear();
    }
}