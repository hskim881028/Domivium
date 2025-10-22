using Domivium.Client.Core.Actors;
using Domivium.Client.Core.State;

namespace Domivium.Client.Core.Message
{
    public readonly struct ActorStateMessage
    {
        public StateTag Tag { get; }
        public ushort Uid { get; }
        public ActorId ActorId { get; }
        public int Id { get; }

        private ActorStateMessage(
            StateTag tag,
            ushort uid,
            ActorId actorId,
            int id)
        {
            Tag = tag;
            Uid = uid;
            ActorId = actorId;
            Id = id;
        }

        public static ActorStateMessage Create(StateTag tag, IActorPresenter presenter)
            => new(tag, presenter.Uid, presenter.ActorId, presenter.Id);
    }
}