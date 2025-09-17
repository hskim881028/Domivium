using System;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Battle;

namespace Domivium.Client.Core.Message
{
    public readonly struct ActorTagMessage
    {
        public ActorTag Tag { get; }
        public Guid Id { get; }

        private ActorTagMessage(ActorTag tag, Guid id)
        {
            Tag = tag;
            Id = id;
        }

        public static ActorTagMessage Create(ActorTag tag, Guid id) => new(tag, id);
    }
}