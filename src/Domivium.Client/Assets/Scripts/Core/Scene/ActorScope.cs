using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Actors.Contract;
using VContainer.Unity;

namespace Domivium.Client.Core.Scene
{
    public class ActorScope : LifetimeScope
    {
        private bool _isDespawn;

        public ushort Uid { get; private set; }
        public IActorPresenter Presenter { get; private set; }

        public void Initialize(ushort uid, ActorId actorId, IActorPresenter presenter)
        {
            Uid = uid;
            Presenter = presenter;
            Presenter.Initialize(uid, actorId, transform);
        }

        public async UniTask SpawnAsync(ActorParam param, CancellationToken token)
        {
            _isDespawn = false;
            gameObject.SetActive(true);
            await Presenter.SpawnAsync(token, param);
            Presenter.Activate();
        }

        public void Despawn()
        {
            if (_isDespawn) return;

            _isDespawn = true;
            Presenter.Despawn();
            gameObject.SetActive(false);
        }
    }
}