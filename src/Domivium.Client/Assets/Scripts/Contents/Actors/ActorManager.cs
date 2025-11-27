using System;
using System.Collections.Generic;
using Domivium.Client.Contents.State;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Message;
using MessagePipe;
using R3;

namespace Domivium.Client.Contents.Actors
{
    public sealed class ActorManager : Disposable, IActorManager
    {
        private readonly Dictionary<ushort, (ActorId actorId, Action<ushort> onDespawn)> _index = new();
        private readonly Dictionary<ActorId, ActorBucket> _buckets = new();
        private readonly Dictionary<ushort, ActorId> _pendingRemove = new();
        private readonly Dictionary<ushort, Action<ushort>> _awaitDespawn = new();
        private readonly List<ActorBucket> _bucketSnapshot = new(32);
        private readonly ReactiveProperty<IUnitPresenter> _character = new();

        public ReadOnlyReactiveProperty<IUnitPresenter> Character => _character;

        public ActorManager(
            ISubscriber<SceneMessage> sceneSubscriber,
            ISubscriber<SpawnActorMessage> spawnActorSubscriber,
            ISubscriber<ActorStateMessage> actorStateSubscriber)
        {
            sceneSubscriber.Subscribe(OnSceneMessage).AddTo(ref DisposableBag);
            spawnActorSubscriber.Subscribe(OnSpawnActorMessage).AddTo(ref DisposableBag);
            actorStateSubscriber.Subscribe(OnActorStateMessage).AddTo(ref DisposableBag);
        }

        public bool Any(ActorId actorId)
        {
            if (!_buckets.TryGetValue(actorId, out var bucket)) return false;

            var removeCount = 0;
            foreach (var kv in _pendingRemove)
            {
                if (kv.Value == actorId)
                {
                    removeCount++;
                }
            }

            return bucket.Count > removeCount;
        }

        public bool TryGet(ActorId actorId, ushort uid, out IActorPresenter presenter)
        {
            presenter = null;
            return _buckets.TryGetValue(actorId, out var bucket) && bucket.TryGet(uid, out presenter);
        }

        public bool TryGetAll(ActorId actorId, out IReadOnlyDictionary<ushort, IActorPresenter> map)
        {
            if (_buckets.TryGetValue(actorId, out var b))
            {
                map = b.Map;
                return true;
            }
            map = null;
            return false;
        }

        public bool TryGetPawn(ActorId actorId, ushort uid, out IBattleSystem pawn)
        {
            pawn = null;
            return _buckets.TryGetValue(actorId, out var bucket) && bucket.TryGetPawn(uid, out pawn);
        }

        public int GetPawns(ActorId actorId, List<IBattleSystem> buffer) => !_buckets.TryGetValue(actorId, out var bucket) ? 0 : bucket.CollectPawns(buffer);

        public int GetPawns(ReadOnlySpan<ActorId> actorIds, List<IBattleSystem> buffer)
        {
            var total = 0;
            foreach (var actorId in actorIds)
            {
                if (_buckets.TryGetValue(actorId, out var bucket))
                {
                    total += bucket.CollectPawns(buffer);
                }
            }
            return total;
        }

        public void Tick(float deltaTime)
        {
            if (_pendingRemove.Count > 0)
            {
                foreach (var id in _pendingRemove.Keys)
                {
                    if (!_index.Remove(id, out var meta)) continue;

                    if (_buckets.TryGetValue(meta.actorId, out var bucket))
                    {
                        bucket.Remove(id);
                    }

                    if (meta.onDespawn != null)
                    {
                        _awaitDespawn[id] = meta.onDespawn;
                    }
                }
                _pendingRemove.Clear();
            }

            _bucketSnapshot.Clear();
            foreach (var bucket in _buckets.Values)
            {
                _bucketSnapshot.Add(bucket);
            }

            foreach (var bucket in _bucketSnapshot)
            {
                bucket.TickAll(deltaTime);
            }
        }

        private ActorBucket GetOrAddBucket(ActorId actorId)
        {
            if (_buckets.TryGetValue(actorId, out var bucket)) return bucket;

            bucket = new ActorBucket();
            _buckets.Add(actorId, bucket);
            return bucket;
        }

        private void TerminateAll()
        {
            foreach (var bucket in _buckets.Values)
            {
                bucket.Terminate();
            }

            _character.Value = null;
            _buckets.Clear();
            _index.Clear();
            _pendingRemove.Clear();
            _awaitDespawn.Clear();
            _bucketSnapshot.Clear();
        }

        private void OnSceneMessage(SceneMessage message)
        {
            switch (message.Type)
            {
                case SceneMessageType.Unload:
                    TerminateAll();
                    break;
                case SceneMessageType.Load:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnSpawnActorMessage(SpawnActorMessage message)
        {
            if (message.Presenter is CharacterPresenter character)
            {
                _character.Value = character;
            }

            if (!_index.TryAdd(message.Uid, (message.ActorId, message.OnDespawn)))
            {
                throw new InvalidOperationException($"Actor already exists: {message.Uid}");
            }

            GetOrAddBucket(message.ActorId).Add(message.Uid, message.Presenter);
        }

        private void OnActorStateMessage(ActorStateMessage message)
        {
            if (message.Tag == StateTags.Die)
            {
                _pendingRemove.TryAdd(message.Uid, message.ActorId);
                return;
            }

            if (message.Tag == StateTags.Despawn)
            {
                if (_awaitDespawn.Remove(message.Uid, out var cb))
                {
                    cb?.Invoke(message.Uid);
                }
            }
        }
    }
}