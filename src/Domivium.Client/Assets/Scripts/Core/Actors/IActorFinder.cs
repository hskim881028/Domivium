using System;
using Domivium.Client.Core.Battle;
using UnityEngine;

namespace Domivium.Client.Core.Actors
{
    public interface IActorFinder
    {
        public bool IsExistUnit(ActorId actorId);
        public bool FindTarget(ActorId actorId, ushort id, out IBattleSystem target);
        public bool FindChaseTarget(ActorId actorId, IBattleSystem source, out IBattleSystem target, out Vector3 chasePosition);
        public bool FindNearestBattleTarget(ActorId actorId, IBattleSystem source, out IBattleSystem target);
        public bool RecalculateChasePosition(IBattleSystem source, IBattleSystem target, out Vector3 chasePosition);
    }
}