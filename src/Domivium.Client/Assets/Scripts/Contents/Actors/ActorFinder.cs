using System.Collections.Generic;
using Domivium.Client.Contents.Battle;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Battle;
using Domivium.Client.Data.Stat;
using UnityEngine;
using UnityEngine.AI;

namespace Domivium.Client.Contents.Actors
{
    public sealed class ActorFinder : IActorFinder
    {
        private const float Revision = 0.1f;
        private readonly ActorManager _actorManager;
        private readonly NavMeshPath _path = new();
        private readonly Vector3[] _cornerBuffer = new Vector3[64];

        public ActorFinder(ActorManager actorManager)
        {
            _actorManager = actorManager;
        }

        public bool IsExistUnit(ActorId actorId) => _actorManager.IsExistUnit(actorId);

        public bool FindTarget(ActorId actorId, ushort id, out IBattleSystem target)
        {
            target = null;
            if (_actorManager.TryGetUnit(actorId, id, out var presenter))
            {
                target = presenter.BattleSystem;
            }

            return target != null;
        }

        public bool TryGetChasePosition(IBattleSystem source, IBattleSystem target, out Vector3 chasePosition)
        {
            var offsetX = CalculateAttackOffset(source, target);
            return TryCalculateChasePath(source.UnitPosition, target.UnitPosition, offsetX, out chasePosition);
        }

        public bool FindChaseTarget(
            ActorId actorId,
            IBattleSystem source,
            out IBattleSystem target,
            out Vector3 chasePosition)
        {
            target = null;
            chasePosition = Vector3.zero;
            if (!_actorManager.TryGetUnits(actorId, out var units)) return false;

            var bestPathLen = float.PositiveInfinity;
            var sourcePosition = source.UnitPosition;
            var detectionRange = source.Stat.RateValue(StatId.DetectionRange);
            foreach (var newTarget in GetUnitsWithinRadius(units, sourcePosition, detectionRange))
            {
                if (source.Id == newTarget.Id) continue;

                var targetPosition = newTarget.UnitPosition;
                var offsetX = CalculateAttackOffset(source, newTarget);
                if (!TryCalculateChasePath(sourcePosition, targetPosition, offsetX, out var newChasePosition)) continue;

                var pathLen = MeasurePathLength(_path, _cornerBuffer, bestPathLen);
                if (pathLen >= bestPathLen) continue;

                bestPathLen = pathLen;
                target = newTarget;
                chasePosition = newChasePosition;
            }

            return target != null;
        }

        public bool FindNearestBattleTarget(
            ActorId actorId,
            IBattleSystem source,
            out IBattleSystem target)
        {
            target = null;
            if (!_actorManager.TryGetUnits(actorId, out var units)) return false;

            var bestDistance = float.PositiveInfinity;
            var sourcePosition = source.UnitPosition;
            var attackRange = source.Stat.RateValue(StatId.AttackRange);
            foreach (var newTarget in GetUnitsWithinRadius(units, sourcePosition, attackRange))
            {
                if (source.Id == newTarget.Id) continue;

                if (!BattleCalculator.CanBattle(source, newTarget)) continue;

                var distance = Vector3.Distance(source.UnitPosition, newTarget.UnitPosition);

                if (distance > bestDistance) continue;

                bestDistance = distance;
                target = newTarget;
            }

            return target != null;
        }

        public bool RecalculateChasePosition(IBattleSystem source, IBattleSystem target, out Vector3 chasePosition)
        {
            chasePosition = Vector3.zero;
            var offsetX = CalculateAttackOffset(source, target);
            if (!TryCalculateChasePath(source.UnitPosition, target.UnitPosition, offsetX, out var newChasePosition)) return false;

            chasePosition = newChasePosition;
            return true;
        }

        private static IEnumerable<IBattleSystem> GetUnitsWithinRadius(IReadOnlyDictionary<ushort, IUnitPresenter> units, Vector3 sourcePosition, float range)
        {
            var r2 = range * range;
            foreach (var unit in units.Values)
            {
                var target = unit.BattleSystem;
                var targetPosition = target.UnitPosition;
                var d2 = (targetPosition - sourcePosition).sqrMagnitude;
                if (d2 > r2) continue;

                yield return target;
            }
        }

        private float CalculateAttackOffset(IBattleSystem source, IBattleSystem target)
        {
            if (source.Type == UnitTypes.Ranged || source.Type == UnitTypes.Support) return 0;

            return source.Stat.RateValue(StatId.AttackRange) + target.Stat.RateValue(StatId.HitRange) - Revision;
        }

        private bool TryCalculateChasePath(Vector3 source, Vector3 target, float offsetX, out Vector3 chasePosition)
        {
            chasePosition = Vector3.zero;
            if (offsetX > 0)
            {
                var right = target;
                right.x += offsetX;
                var rightDist = Vector3.Distance(right, source);

                var left = target;
                left.x -= offsetX;
                var leftDist = Vector3.Distance(left, source);

                if (leftDist > rightDist)
                {
                    if (!NavMesh.CalculatePath(source, right, NavMesh.AllAreas, _path))
                    {
                        if (!NavMesh.CalculatePath(source, left, NavMesh.AllAreas, _path)) return false;

                        chasePosition = left;
                    }

                    chasePosition = right;
                }
                else
                {
                    if (!NavMesh.CalculatePath(source, left, NavMesh.AllAreas, _path))
                    {
                        if (!NavMesh.CalculatePath(source, right, NavMesh.AllAreas, _path)) return false;

                        chasePosition = right;
                    }

                    chasePosition = left;
                }
            }
            else
            {
                if (!NavMesh.CalculatePath(source, target, NavMesh.AllAreas, _path)) return false;

                chasePosition = target;
            }

            return _path.status == NavMeshPathStatus.PathComplete;
        }

        private static float MeasurePathLength(NavMeshPath path, Vector3[] cornerBuffer, float cutoffLength)
        {
            var corners = path.GetCornersNonAlloc(cornerBuffer);

            if (corners <= 1) return 0;

            if (corners == cornerBuffer.Length) return float.PositiveInfinity;

            var length = 0f;
            for (var i = 1; i < corners; i++)
            {
                length += Vector3.Distance(cornerBuffer[i - 1], cornerBuffer[i]);
                if (length >= cutoffLength) break;
            }

            return length;
        }
    }
}