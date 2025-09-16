using System;
using System.Collections.Generic;
using System.Linq;
using Domivium.Client.Contents.Actors.Generated;
using Domivium.Client.Contents.Battle;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Message;
using MessagePipe;
using R3;
using UnityEngine.InputSystem;

namespace Domivium.Client.Contents.Services
{
    public sealed class UnitService : Disposable
    {
        private readonly Dictionary<Guid, IUnitPresenter> _character = new();
        private readonly Dictionary<Guid, IUnitPresenter> _tower = new();
        private readonly Dictionary<Guid, IUnitPresenter> _monster = new();

        public UnitService(ISubscriber<SpawnerMessage> subscriber)
        {
            subscriber.Subscribe(OnSpawnerMessage).AddTo(ref DisposableBag);
        }

        public void Tick(float deltaTime)
        {
            ForTest();
            foreach (var (_, character) in _character)
            {
                character.Tick(deltaTime);
            }

            foreach (var (_, tower) in _tower)
            {
                tower.Tick(deltaTime);
            }

            foreach (var (_, monster) in _monster)
            {
                monster.Tick(deltaTime);
            }
        }

        private void Add(ActorId actorId, Guid scopeId, IUnitPresenter presenter)
        {
            if (actorId == ActorIds.Character)
            {
                if (!_character.TryAdd(scopeId, presenter))
                {
                    throw new Exception($"Character already exists: {scopeId}");
                }
            }
            else if (actorId == ActorIds.Tower)
            {
                if (!_tower.TryAdd(scopeId, presenter))
                {
                    throw new Exception($"Tower already exists: {scopeId}");
                }
            }
            else if (actorId == ActorIds.Monster)
            {
                if (!_monster.TryAdd(scopeId, presenter))
                {
                    throw new Exception($"Monster already exists: {scopeId}");
                }
            }
        }

        private void Remove(ActorId actorId, Guid scopeId)
        {
            if (actorId == ActorIds.Character)
            {
                _character.Remove(scopeId);
            }
            else if (actorId == ActorIds.Tower)
            {
                _tower.Remove(scopeId);
            }
            else if (actorId == ActorIds.Monster)
            {
                _monster.Remove(scopeId);
            }
        }

        private void OnSpawnerMessage(SpawnerMessage message)
        {
            switch (message.Type)
            {
                case SpawnerMessageType.Spawn:
                    if (message.Presenter is IUnitPresenter presenter)
                    {
                        Add(message.ActorId, message.ScopeId, presenter);
                    }

                    break;
                case SpawnerMessageType.Despawn:
                    Remove(message.ActorId, message.ScopeId);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ForTest()
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                _character.First().Value.BattleSystem.TryActivateAbility(BattleAbilityIds.Slash);
            }
        }
    }
}