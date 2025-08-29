using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Domivium.Client.Core.Message;
using Domivium.Client.Core.Scene;
using Domivium.Client.Core.UI;
using MessagePipe;
using R3;
using VContainer;
using VContainer.Unity;
using DisposableBag = R3.DisposableBag;
using Object = UnityEngine.Object;

namespace Domivium.Client.Core.Actors
{
    public sealed class ActorSpawner : IActorSpawner
    {
        private const int MaxPoolPerActor = 16;

        private ActorRootScope _root;

        private readonly Dictionary<ActorId, (Type presenter, Type view)> _container;
        private readonly List<Actor> _prefabs;
        private readonly Dictionary<ActorId, Queue<ActorScope>> _pool = new();

        private DisposableBag _disposable;
        private bool _isDisposed;
        private SceneMessageType _sceneMessageType;

        public ActorSpawner(
            Dictionary<ActorId, (Type presenter, Type view)> container,
            List<Actor> prefabs,
            ISubscriber<SceneMessage> subscriber)
        {
            _container = container;
            _prefabs = prefabs;
            subscriber.Subscribe(OnSceneMessage).AddTo(ref _disposable);
        }

        public void Dispose()
        {
            if (_isDisposed) return;

            _isDisposed = true;
            _disposable.Dispose();
        }

        public async UniTask SpawnAsync(ActorId id)
        {
            if (TryGet(id, out var scope))
            {
                await scope.SpawnAsync();
                return;
            }

            var (presenterType, viewType) = _container[id];
            var child = _root.CreateChild<ActorScope>(builder =>
                {
                    var prefab = _prefabs.FirstOrDefault(p => viewType.IsAssignableFrom(p.GetType()));
                    if (prefab == null)
                    {
                        throw new InvalidOperationException($"Actor prefab not found for view type {viewType.FullName}. Make sure it is listed in ActorContainer.");
                    }
                    builder.RegisterComponentInNewPrefab(prefab, Lifetime.Singleton).AsSelf();
                    builder.Register(presenterType, Lifetime.Singleton);
                },
                $"{presenterType.Name.AsActor()}(Scope)");

            var presenter = (IActorPresenter)child.Container.Resolve(presenterType);
            child.Initialize(id, presenter, Despawn);
            await child.SpawnAsync();
        }

        private void OnSceneMessage(SceneMessage message)
        {
            _sceneMessageType = message.Type;
            switch (_sceneMessageType)
            {
                case SceneMessageType.Unload:
                    _pool.Clear();
                    break;
                case SceneMessageType.Load:
                    _root = message.SceneScope.ActorRootScope;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private bool TryGet(ActorId id, out ActorScope scope)
        {
            if (!_pool.TryGetValue(id, out var queue))
            {
                _pool.Add(id, new Queue<ActorScope>());
                queue = _pool[id];
            }

            if (queue.Count > 0)
            {
                scope = queue.Dequeue();
                return true;
            }

            scope = null;
            return false;
        }

        private void Despawn(ActorScope scope)
        {
            if (_sceneMessageType == SceneMessageType.Unload)
            {
                Object.Destroy(scope.gameObject);
                return;
            }

            if (!_pool.TryGetValue(scope.ActorId, out var queue))
            {
                _pool.Add(scope.ActorId, new Queue<ActorScope>());
                queue = _pool[scope.ActorId];
            }

            if (queue.Count < MaxPoolPerActor)
            {
                queue.Enqueue(scope);
            }
            else
            {
                Object.Destroy(scope.gameObject);
            }
        }
    }
}