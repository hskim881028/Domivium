using System;
using Cysharp.Threading.Tasks;
using R3;

namespace Domivium.Client.Core.State
{
    public interface IStateSystem : IDisposable
    {
        public ReadOnlyReactiveProperty<StateTag> Tag { get; }
        public void TryTransit(StateTag next);
        public void Activate();
        public UniTaskVoid DespawnAsync(float despawnSeconds = 0);
        public void Terminate();
    }
}