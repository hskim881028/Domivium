using System;
using Domivium.Client.Core.State;

namespace Domivium.Client.Core.Message
{
    public readonly struct ActorStateMessage
    {
        public StateTag Tag { get; }
        public Guid Id { get; }

        private ActorStateMessage(StateTag tag, Guid id)
        {
            Tag = tag;
            Id = id;
        }

        public static ActorStateMessage Create(StateTag tag, Guid id) => new(tag, id);
    }
}