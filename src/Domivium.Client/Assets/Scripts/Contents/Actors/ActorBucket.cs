using System;
using System.Collections.Generic;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Battle;

namespace Domivium.Client.Contents.Actors
{
    public sealed class ActorBucket
    {
        private readonly Dictionary<ushort, IActorPresenter> _map = new();
        private readonly List<IActorPresenter> _snapshot = new(32);

        public int Count => _map.Count;
        public IReadOnlyDictionary<ushort, IActorPresenter> Map => _map;

        public bool TryGet(ushort id, out IActorPresenter p) => _map.TryGetValue(id, out p);

        public bool TryGetPawn(ushort id, out IBattleSystem unit)
        {
            unit = null;
            if (!TryGet(id, out var presenter)) return false;

            if (presenter is not IPawnPresenter pawnPresenter) return false;

            unit = pawnPresenter.BattleSystem;
            return true;
        }

        public int CollectPawns(List<IBattleSystem> buffer)
        {
            buffer.Clear();
            foreach (var (_, presenter) in _map)
            {
                if (presenter is not IPawnPresenter pawn) continue;

                buffer.Add(pawn.BattleSystem);
            }
            return buffer.Count;
        }

        public void Remove(ushort id)
        {
            if (_map.Remove(id, out var presenter))
            {
                presenter.Terminate();
            }
        }

        public void Add(ushort id, IActorPresenter presenter)
        {
            if (!_map.TryAdd(id, presenter))
            {
                throw new InvalidOperationException($"Presenter already exists: {id}");
            }
        }

        public void TickAll(float deltaTime)
        {
            _snapshot.Clear();
            foreach (var presenter in _map.Values)
            {
                _snapshot.Add(presenter);
            }

            foreach (var presenter in _snapshot)
            {
                presenter.Tick(deltaTime);
            }
        }

        public void Terminate()
        {
            foreach (var presenter in _map.Values)
            {
                presenter.Terminate();
            }

            _map.Clear();
        }
    }
}