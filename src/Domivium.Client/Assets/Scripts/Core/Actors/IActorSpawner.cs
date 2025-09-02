using System;
using Cysharp.Threading.Tasks;
using Domivium.Client.Core.Actors.Contract;

namespace Domivium.Client.Core.Actors
{
    public interface IActorSpawner : IDisposable
    {
        public UniTask<IActorPresenter> SpawnAsync(ActorId id, ActorParam param);
    }
}