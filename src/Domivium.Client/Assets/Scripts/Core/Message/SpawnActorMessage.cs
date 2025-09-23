using System;
using Domivium.Client.Core.Actors;

namespace Domivium.Client.Core.Message
{
    public readonly struct SpawnActorMessage
    {
        public ushort Id { get; }
        public ActorId ActorId { get; }
        public IActorPresenter Presenter { get; }
        public Action<ushort> OnDespawn { get; }

        private SpawnActorMessage(ushort id, ActorId actorId, IActorPresenter presenter, Action<ushort> onDespawn)
        {
            Id = id;
            ActorId = actorId;
            Presenter = presenter;
            OnDespawn = onDespawn;
        }

        public static SpawnActorMessage Create(ushort id, ActorId actorId, IActorPresenter presenter, Action<ushort> onDespawn)
            => new(id, actorId, presenter, onDespawn);
    }
}