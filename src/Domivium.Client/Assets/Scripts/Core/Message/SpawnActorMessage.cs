using System;
using Domivium.Client.Core.Actors;

namespace Domivium.Client.Core.Message
{
    public readonly struct SpawnActorMessage
    {
        public ushort Uid { get; }
        public ActorId ActorId { get; }
        public IActorPresenter Presenter { get; }
        public Action<ushort> OnDespawn { get; }

        private SpawnActorMessage(ushort uid, ActorId actorId, IActorPresenter presenter, Action<ushort> onDespawn)
        {
            Uid = uid;
            ActorId = actorId;
            Presenter = presenter;
            OnDespawn = onDespawn;
        }

        public static SpawnActorMessage Create(ushort uid, ActorId actorId, IActorPresenter presenter, Action<ushort> onDespawn)
            => new(uid, actorId, presenter, onDespawn);
    }
}