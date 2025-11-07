using System;
using System.Collections.Generic;
using Domivium.Client.Core.Battle;

namespace Domivium.Client.Core.Actors
{
    public interface IActorManager : ITicker
    {
        public bool Any(ActorId actorId);
        public bool TryGet(ActorId actorId, ushort uid, out IActorPresenter presenter);
        public bool TryGetAll(ActorId actorId, out IReadOnlyDictionary<ushort, IActorPresenter> map);
        public bool TryGetPawn(ActorId actorId, ushort uid, out IBattleSystem pawn);
        public bool GetCharacter(out IBattleSystem pawn);
        public int GetPawns(ActorId actorId, List<IBattleSystem> buffer);
        public int GetPawns(ReadOnlySpan<ActorId> actorIds, List<IBattleSystem> buffer);
    }
}