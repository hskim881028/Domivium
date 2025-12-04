using Cysharp.Threading.Tasks;
using Domivium.Client.Core.Actors.Contract;

namespace Domivium.Client.Core.Actors
{
    public interface IActorSpawner
    {
        public UniTask<IActorPresenter> SpawnAsync(ActorId actorId, ActorParam param, float delayTime = 0f);
    }
}