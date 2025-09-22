using System;
using Domivium.Client.Core.State;

namespace Domivium.Client.Core.Actors
{
    public interface IActorPresenter : IActorActivatable, ITicker
    {
        public Guid Id { get; }
        public bool CanTransitState(StateTag current, StateTag next);
        public void Terminate();
    }
}