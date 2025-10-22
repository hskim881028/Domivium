using Domivium.Client.Core.State;

namespace Domivium.Client.Core.Actors
{
    public interface IActorPresenter : IActorActivatable, ITicker
    {
        public ushort Uid { get; }
        public ActorId ActorId { get; }
        public int Id { get; }
        public bool CanTransitState(StateTag current, StateTag next);
        public void Terminate();
    }
}