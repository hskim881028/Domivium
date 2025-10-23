using System;
using System.Collections.Generic;
using System.Linq;
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

        public bool TryGet(ushort uid, out IActorPresenter p) => _map.TryGetValue(uid, out p);

        public bool TryGetPawn(ushort uid, out IBattleSystem pawn)
        {
            pawn = null;
            if (!TryGet(uid, out var presenter)) return false;

            if (presenter is not IPawnPresenter pawnPresenter) return false;

            pawn = pawnPresenter.BattleSystem;
            return true;
        }

        public bool TryGetFirstPawn(out IBattleSystem pawn)
        {
            pawn = null;
            if (_map.Count <= 0) return false;

            if (_map.First().Value is not IPawnPresenter pawnPresenter) return false;

            pawn = pawnPresenter.BattleSystem;
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

        public void Remove(ushort uid)
        {
            if (_map.Remove(uid, out var presenter))
            {
                presenter.Terminate();
            }
        }

        public void Add(ushort uid, IActorPresenter presenter)
        {
            if (!_map.TryAdd(uid, presenter))
            {
                throw new InvalidOperationException($"Presenter already exists: {uid}");
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