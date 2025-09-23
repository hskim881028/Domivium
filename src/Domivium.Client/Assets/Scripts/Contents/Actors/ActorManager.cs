using System;
using System.Collections.Generic;
using Domivium.Client.Contents.Context;
using Domivium.Client.Contents.State;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Context;
using Domivium.Client.Core.Message;
using MessagePipe;
using R3;

namespace Domivium.Client.Contents.Actors
{
    public sealed class ActorManager : Disposable
    {
        private readonly Dictionary<ushort, (ActorId actorId, Action<ushort> onDespawn)> _actor = new();
        private readonly Dictionary<ActorId, Dictionary<ushort, IUnitPresenter>> _unit = new();
        private readonly Dictionary<ActorId, Dictionary<ushort, IVFXPresenter>> _vfx = new();
        private readonly Dictionary<ushort, Action<ushort>> _pendingDespawns = new();
        private readonly Queue<ushort> _immediateDespawns = new();
        private readonly Queue<ushort> _pendingRemoves = new();

        public ActorManager(
            StageContext stageContext,
            ISubscriber<SceneMessage> sceneSubscriber,
            ISubscriber<SpawnActorMessage> spawnActorSubscriber,
            ISubscriber<ActorStateMessage> actorTagSubscriber)
        {
            stageContext.Phase.Subscribe(OnChangedPhase).AddTo(ref DisposableBag);
            sceneSubscriber.Subscribe(OnSceneMessage).AddTo(ref DisposableBag);
            spawnActorSubscriber.Subscribe(OnSpawnActorMessage).AddTo(ref DisposableBag);
            actorTagSubscriber.Subscribe(OnActorTagMessage).AddTo(ref DisposableBag);
        }

        public bool IsExistUnit(ActorId actorId) => _unit[actorId].Count > 0;

        public bool TryGetUnit(ActorId actorId, ushort id, out IUnitPresenter unitPresenter)
        {
            if (_unit.TryGetValue(actorId, out var units))
            {
                if (units.TryGetValue(id, out var unit))
                {
                    unitPresenter = unit;
                    return true;
                }
            }

            unitPresenter = null;
            return false;
        }

        public bool TryGetUnits(ActorId actorId, out IReadOnlyDictionary<ushort, IUnitPresenter> units)
        {
            if (_unit.TryGetValue(actorId, out var value))
            {
                units = value;
                return true;
            }

            units = null;
            return false;
        }

        public void Tick(float deltaTime)
        {
            while (_immediateDespawns.Count > 0)
            {
                var id = _immediateDespawns.Dequeue();
                var callback = Remove(id);
                callback.Invoke(id);
            }

            while (_pendingRemoves.Count > 0)
            {
                var id = _pendingRemoves.Dequeue();
                var callback = Remove(id);
                if (callback != null)
                {
                    _pendingDespawns[id] = callback;
                }
            }

            foreach (var dic in _unit.Values)
            {
                foreach (var presenter in dic.Values)
                {
                    presenter.Tick(deltaTime);
                }
            }

            foreach (var dic in _vfx.Values)
            {
                foreach (var presenter in dic.Values)
                {
                    presenter.Tick(deltaTime);
                }
            }
        }

        protected override void OnDispose()
        {
            Clear();
            base.OnDispose();
        }

        private void Clear()
        {
            _actor.Clear();
            _unit.Clear();
            _vfx.Clear();
            _pendingDespawns.Clear();
            _immediateDespawns.Clear();
            _pendingRemoves.Clear();
        }

        private Action<ushort> Remove(ushort id)
        {
            if (!_actor.Remove(id, out var value))
            {
                throw new Exception($"not exists. actor: {id}");
            }

            if (_unit.TryGetValue(value.actorId, out var unit))
            {
                unit.Remove(id);
            }

            if (_vfx.TryGetValue(value.actorId, out var vfx))
            {
                vfx.Remove(id);
            }

            return value.onDespawn;
        }

        private void OnChangedPhase(StagePhase phase)
        {
            if (phase == StagePhases.Failed || phase == StagePhases.Cleared)
            {
                foreach (var (_, presenters) in _unit)
                {
                    foreach (var (_, presenter) in presenters)
                    {
                        presenter.Terminate();
                    }
                    presenters.Clear();
                }

                foreach (var (_, presenters) in _vfx)
                {
                    foreach (var (_, presenter) in presenters)
                    {
                        presenter.Terminate();
                    }
                    presenters.Clear();
                }
            }
        }

        private void OnSceneMessage(SceneMessage message)
        {
            switch (message.Type)
            {
                case SceneMessageType.Unload:
                    Clear();
                    break;
                case SceneMessageType.Load:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnSpawnActorMessage(SpawnActorMessage message)
        {
            var id = message.Id;
            var actorId = message.ActorId;
            if (!_actor.TryAdd(id, (actorId, message.OnDespawn)))
            {
                throw new Exception($"Already exists. actor: {id}");
            }

            switch (message.Presenter)
            {
                case IUnitPresenter unitPresenter:
                    if (!_unit.TryGetValue(actorId, out var unit))
                    {
                        unit = new Dictionary<ushort, IUnitPresenter>();
                        _unit.Add(actorId, unit);
                    }

                    if (!unit.TryAdd(id, unitPresenter))
                    {
                        throw new Exception($"Already exists. unit: {id}");
                    }
                    break;
                case IVFXPresenter vfxPresenter:
                    if (!_vfx.TryGetValue(actorId, out var vfx))
                    {
                        vfx = new Dictionary<ushort, IVFXPresenter>();
                        _vfx.Add(actorId, vfx);
                    }

                    if (!vfx.TryAdd(id, vfxPresenter))
                    {
                        throw new Exception($"Already exists. vfx: {id}");
                    }
                    break;
            }
        }

        private void OnActorTagMessage(ActorStateMessage message)
        {
            if (message.Tag == StateTags.Die)
            {
                _pendingRemoves.Enqueue(message.Id);
                return;
            }

            if (message.Tag == StateTags.Despawn)
            {
                if (_pendingDespawns.Remove(message.Id, out var onDespawn))
                {
                    onDespawn.Invoke(message.Id);
                }
                else
                {
                    _immediateDespawns.Enqueue(message.Id);
                }
            }
        }
    }
}