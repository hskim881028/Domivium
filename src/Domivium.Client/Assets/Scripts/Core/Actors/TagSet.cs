using ObservableCollections;
using R3;

namespace Domivium.Client.Core.Actors
{
    public sealed class TagSet
    {
        private readonly ReactiveProperty<ActorTag> _state = new();
        private readonly ObservableHashSet<ActorTag> _tags = new();

        public ReadOnlyReactiveProperty<ActorTag> State => _state;

        public void SetState(ActorTag tag) => _state.Value = tag;

        public void Subscribe(NotifyCollectionChangedEventHandler<ActorTag> handler) => _tags.CollectionChanged += handler;

        public void Unsubscribe(NotifyCollectionChangedEventHandler<ActorTag> handler) => _tags.CollectionChanged -= handler;

        public bool Contains(ActorTag tag) => _tags.Contains(tag);

        public void Add(ActorTag tag) => _tags.Add(tag);

        public void Remove(ActorTag tag) => _tags.Remove(tag);

        public void Clear()
        {
            _state.Value = default;
            _tags.Clear();
        }
    }
}