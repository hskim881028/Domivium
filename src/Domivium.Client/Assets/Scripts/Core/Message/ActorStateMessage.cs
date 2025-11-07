using Domivium.Client.Core.Actors;
using Domivium.Client.Core.State;

namespace Domivium.Client.Core.Message
{
    public readonly struct ActorStateMessage
    {
        public StateTag Tag { get; }
        public ushort Uid { get; }
        public ActorId ActorId { get; }

        private ActorStateMessage(StateTag tag, ushort uid, ActorId actorId)
        {
            Tag = tag;
            Uid = uid;
            ActorId = actorId;
        }

        public static ActorStateMessage Create(StateTag tag, IActorPresenter presenter)
            => new(tag, presenter.Uid, presenter.ActorId);
    }
}