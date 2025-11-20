using Domivium.Client.Core.State;
using UnityEngine;

namespace Domivium.Client.Core.Actors
{
    public interface IActorPresenter : IActorActivatable, ITicker
    {
        public ushort Uid { get; }
        public ActorId ActorId { get; }
        public Transform Transform { get; }
        public bool CanTransitState(StateTag current, StateTag next);
        public void Terminate();
    }
}