using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Actors.Contract;
using VContainer.Unity;

namespace Domivium.Client.Core.Scene
{
    public class ActorScope : LifetimeScope
    {
        private IActorPresenter _presenter;
        private Action<ActorScope> _onDespawn;
        private CancellationTokenSource _cts = new();
        private bool _isDespawn;

        public ActorId ActorId { get; private set; }

        public void Initialize(ActorId id, IActorPresenter presenter, Action<ActorScope> onDespawn)
        {
            ActorId = id;
            _presenter = presenter;
            _presenter.Initialize(transform, Despawn);
            _onDespawn = onDespawn;
        }

        public async UniTask<IActorPresenter> SpawnAsync(ActorParam param)
        {
            _isDespawn = false;
            gameObject.SetActive(true);
            await _presenter.ShowAsync(_cts.Token, param);
            return _presenter;
        }

        private void Despawn()
        {
            if (_isDespawn) return;

            _isDespawn = true;
            _cts.Cancel();
            _cts.Dispose();
            _cts = new CancellationTokenSource();
            gameObject.SetActive(false);
            _onDespawn?.Invoke(this);
        }

        protected override void OnDestroy()
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
            base.OnDestroy();
        }
    }
}