using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Battle;
using UnityEngine;

namespace Domivium.Client.Contents.ReadModels
{
    public interface IBattleService
    {
        public Vector3 GetPositionOffset(IBattleSystem source, IBattleSystem target);
        public bool FindTarget(ActorId actorId, ushort uid, out IBattleSystem target);
        public bool FindChaseTarget(ActorId actorId, IBattleSystem source, out IBattleSystem target, out Vector3 chasePosition);
        public bool FindNearestBattleTarget(ActorId actorId, IBattleSystem source, out IBattleSystem target);
        public bool RecalculateChasePosition(IBattleSystem source, IBattleSystem target, out Vector3 chasePosition);
        public bool TryGetChasePosition(IBattleSystem source, IBattleSystem target, out Vector3 chasePosition);
    }
}