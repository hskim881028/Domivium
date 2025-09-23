using Domivium.Client.Core.State;

namespace Domivium.Client.Core.Message
{
    public readonly struct ActorStateMessage
    {
        public StateTag Tag { get; }
        public ushort Id { get; }

        private ActorStateMessage(StateTag tag, ushort id)
        {
            Tag = tag;
            Id = id;
        }

        public static ActorStateMessage Create(StateTag tag, ushort id) => new(tag, id);
    }
}