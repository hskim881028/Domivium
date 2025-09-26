using Domivium.Client.Core.Actors;
using Domivium.Client.Core.State;

namespace Domivium.Client.Core.Message
{
    public readonly struct ActorStateMessage
    {
        public StateTag Tag { get; }
        public ushort Id { get; }
        public ActorId ActorId { get; }

        private ActorStateMessage(StateTag tag, ushort id, ActorId actorId)
        {
            Tag = tag;
            Id = id;
            ActorId = actorId;
        }

        public static ActorStateMessage Create(StateTag tag, ushort id, ActorId actorId) => new(tag, id, actorId);
    }
}