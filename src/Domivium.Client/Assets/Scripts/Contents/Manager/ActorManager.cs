using System;
using System.Collections.Generic;
using System.Linq;
using Domivium.Client.Contents.Actors.Generated;
using Domivium.Client.Contents.Battle;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Message;
using Domivium.Client.Core.Utility;
using MessagePipe;
using R3;
using UnityEngine.InputSystem;

namespace Domivium.Client.Contents.Manager
{
    public sealed class ActorManager : Disposable
    {
        private readonly Dictionary<Guid, (ActorId actorId, Action<Guid> onDespawn)> _actor = new();
        private readonly Dictionary<ActorId, Dictionary<Guid, IUnitPresenter>> _unit = new();
        private readonly Dictionary<ActorId, Dictionary<Guid, IVFXPresenter>> _vfx = new();
        private readonly Dictionary<Guid, Action<Guid>> _pendingDespawns = new();
        private readonly Queue<Guid> _immediateDespawns = new();
        private readonly Queue<Guid> _pendingRemoves = new();

        public ActorManager(
            ISubscriber<SpawnActorMessage> spawnActorSubscriber,
            ISubscriber<ActorTagMessage> actorTagSubscriber)
        {
            spawnActorSubscriber.Subscribe(OnSpawnActorMessage).AddTo(ref DisposableBag);
            actorTagSubscriber.Subscribe(OnActorTagMessage).AddTo(ref DisposableBag);
        }

        public void Tick(float deltaTime)
        {
            ForTest();

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
                _pendingDespawns.Add(id, callback);
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
                    if (!_unit.ContainsKey(actorId))
                    {
                        _unit.Add(actorId, new Dictionary<Guid, IUnitPresenter>());
                    }

                    if (!_unit[actorId].TryAdd(id, unitPresenter))
                    {
                        throw new Exception($"Already exists. unit: {id}");
                    }
                    break;
                case IVFXPresenter vfxPresenter:
                    if (!_vfx.ContainsKey(actorId))
                    {
                        _vfx.Add(actorId, new Dictionary<Guid, IVFXPresenter>());
                    }

                    if (!_vfx[actorId].TryAdd(id, vfxPresenter))
                    {
                        throw new Exception($"Already exists. vfx: {id}");
                    }
                    break;
            }
        }

        private void OnActorTagMessage(ActorTagMessage message)
        {
            if (message.Tag == ActorTag.Die)
            {
                _pendingRemoves.Enqueue(message.Id);
            }
            else if (message.Tag == ActorTag.Despawn)
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

        private Action<Guid> Remove(Guid id)
        {
            _actor.Remove(id, out var value);

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

        private void ForTest()
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                if (_unit[ActorIds.Character].Count > 0)
                {
                    _unit[ActorIds.Character].First().Value.BattleSystem.TryActivateAbility(BattleAbilityIds.Slash);
                }
            }
        }
    }
}