using Cysharp.Threading.Tasks;
using Domivium.Client.Core.Actors.Contract;

namespace Domivium.Client.Core.Actors
{
    public interface IActorSpawner
    {
        public UniTask SpawnAsync(ActorId id, ActorParam param);
    }
}