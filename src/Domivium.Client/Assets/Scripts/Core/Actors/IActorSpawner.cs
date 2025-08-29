using System;
using Cysharp.Threading.Tasks;

namespace Domivium.Client.Core.Actors
{
    public interface IActorSpawner : IDisposable
    {
        public UniTask SpawnAsync(ActorId id);
    }
}