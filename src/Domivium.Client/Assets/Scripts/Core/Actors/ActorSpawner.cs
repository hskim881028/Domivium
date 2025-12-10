using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Core.Actors.Contract;
using Domivium.Client.Core.Message;
using Domivium.Client.Core.Scene;
using Domivium.Client.Core.UI;
using MessagePipe;
using R3;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace Domivium.Client.Core.Actors
{
    public sealed class ActorSpawner : Disposable, IActorSpawner
    {
        private const int MaxPoolPerActor = 16;

        private readonly Dictionary<ActorId, (Type presenter, Type view)> _container;
        private readonly Dictionary<Type, Actor> _prefabs = new();
        private readonly IPublisher<SpawnActorMessage> _publisher;
        private readonly Dictionary<ActorId, Queue<(ushort uid, ActorScope scope)>> _pool = new();
        private readonly Dictionary<ushort, (ActorId actorId, ActorScope scope)> _activeActors = new();

        private ActorRootScope _root;
        private SceneMessageType _sceneMessageType;
        private CancellationTokenSource _cts = new();
        private ushort _uid;

        public ActorSpawner(
            Dictionary<ActorId, (Type presenter, Type view)> container,
            List<Actor> prefabs,
            IPublisher<SpawnActorMessage> publisher,
            ISubscriber<SceneMessage> sceneSubscriber)
        {
            _container = container;
            foreach (var prefab in prefabs)
            {
                _prefabs[prefab.GetType()] = prefab;
            }

            _publisher = publisher;
            sceneSubscriber.Subscribe(OnSceneMessage).AddTo(ref DisposableBag);
        }

        public async UniTask<IActorPresenter> SpawnAsync(ActorId actorId, ActorParam param, float delayTime = 0f)
        {
            await Awaitable.WaitForSecondsAsync(delayTime);

            if (TryGet(actorId, out var actor))
            {
                await SpawnInternalAsync(actor.scope, actor.scope.Presenter, actor.uid, actorId, param);
                return actor.scope.Presenter;
            }

            var actorScope = CreateActor(actorId);
            await SpawnInternalAsync(actorScope, actorScope.Presenter, actorScope.Uid, actorId, param);
            return actorScope.Presenter;
        }

        private ActorScope CreateActor(ActorId actorId)
        {
            var (presenterType, viewType) = _container[actorId];

            var actorScope = _root.CreateChild<ActorScope>(builder =>
                {
                    if (!_prefabs.TryGetValue(viewType, out var prefab))
                    {
                        throw new InvalidOperationException($"Actor prefab not found for view type {viewType.FullName}. Check ActorContainer list.");
                    }

                    builder.RegisterComponentInNewPrefab(prefab, Lifetime.Singleton).AsSelf();
                    builder.Register(presenterType, Lifetime.Singleton);
                },
                $"{presenterType.Name.AsActor()}(Scope)");

            _uid++;
            var presenter = (IActorPresenter)actorScope.Container.Resolve(presenterType);
            actorScope.Initialize(_uid, actorId, presenter);
            return actorScope;
        }

        protected override void OnDispose()
        {
            Clear();
            base.OnDispose();
        }

        private async UniTask SpawnInternalAsync(
            ActorScope scope,
            IActorPresenter presenter,
            ushort uid,
            ActorId actorId,
            ActorParam param)
        {
            await scope.SpawnAsync(param, _cts.Token);
            _activeActors.Add(uid, (actorId, scope));
            _publisher.Publish(SpawnActorMessage.Create(uid, actorId, presenter, Return));
        }

        private void Return(ushort uid)
        {
            if (_sceneMessageType == SceneMessageType.Unload) return;

            if (!_activeActors.Remove(uid, out var value))
            {
                throw new InvalidOperationException($"Actor scope not found for scope: {uid}");
            }

            if (!_pool.ContainsKey(value.actorId))
            {
                _pool.Add(value.actorId, new Queue<(ushort, ActorScope)>());
            }

            if (_pool[value.actorId].Count < MaxPoolPerActor)
            {
                value.scope.Despawn();
                _pool[value.actorId].Enqueue((uid, value.scope));
            }
            else
            {
                Object.Destroy(value.scope.gameObject);
            }
        }

        private void OnSceneMessage(SceneMessage message)
        {
            _sceneMessageType = message.Type;
            switch (_sceneMessageType)
            {
                case SceneMessageType.Unload:
                    Clear();
                    _cts = new CancellationTokenSource();
                    break;
                case SceneMessageType.Load:
                    _cts = new CancellationTokenSource();
                    _root = message.SceneScope.ActorRootScope;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private bool TryGet(ActorId actorId, out (ushort uid, ActorScope scope) actor)
        {
            if (!_pool.TryGetValue(actorId, out var queue))
            {
                _pool.Add(actorId, new Queue<(ushort, ActorScope)>());
                queue = _pool[actorId];
            }

            if (queue.Count > 0)
            {
                actor = queue.Dequeue();
                return true;
            }

            actor = default;
            return false;
        }

        private void Clear()
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
            _uid = 0;
            _activeActors.Clear();
            _pool.Clear();
        }
    }
}