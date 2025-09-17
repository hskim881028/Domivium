using System;
using Domivium.Client.Core.Actors;

namespace Domivium.Client.Core.Message
{
    public readonly struct SpawnActorMessage
    {
        public Guid Id { get; }
        public ActorId ActorId { get; }
        public IActorPresenter Presenter { get; }
        public Action<Guid> OnDespawn { get; }

        private SpawnActorMessage(Guid id, ActorId actorId, IActorPresenter presenter, Action<Guid> onDespawn)
        {
            Id = id;
            ActorId = actorId;
            Presenter = presenter;
            OnDespawn = onDespawn;
        }

        public static SpawnActorMessage Create(Guid id, ActorId actorId, IActorPresenter presenter, Action<Guid> onDespawn)
            => new(id, actorId, presenter, onDespawn);
    }
}