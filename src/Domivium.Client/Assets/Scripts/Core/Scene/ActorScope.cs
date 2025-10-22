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

        public void Initialize(ushort uid, IActorPresenter presenter)
        {
            Uid = uid;
            Presenter = presenter;
            Presenter.Initialize(uid, transform);
        }

        public async UniTask SpawnAsync(ActorParam param, CancellationToken token)
        {
            _isDespawn = false;
            gameObject.SetActive(true);
            await Presenter.ActivateAsync(token, param);
        }

        public void Despawn()
        {
            if (_isDespawn) return;

            _isDespawn = true;
            Presenter.Deactivate();
            gameObject.SetActive(false);
        }
    }
}