using System.Collections.Generic;
using Domivium.Client.Contents.Actors.Generated;
using Domivium.Client.Contents.Battle;
using Domivium.Client.Contents.ReadModels;
using Domivium.Client.Contents.State;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Battle;
using Domivium.Client.Data.Stat;
using Domivium.Client.Data.Store;
using UnityEngine;
using UnityEngine.AI;

namespace Domivium.Client.Contents.Services
{
    public sealed class BattleService : IBattleService
    {
        private const int TryCount = 32;
        private readonly IActorManager _actorManager;
        private readonly CoordinateService _coordinateService;
        private readonly IStageMapStore _stageMapStore;
        private readonly NavMeshPath _path = new();
        private readonly Vector3[] _cornerBuffer = new Vector3[64];

        private readonly List<IBattleSystem> _units = new(256);

        public BattleService(
            CoordinateService coordinateService,
            IActorManager actorManager,
            IStageMapStore stageMapStore)
        {
            _actorManager = actorManager;
            _coordinateService = coordinateService;
            _stageMapStore = stageMapStore;
        }

        public bool IsExistUnit(ActorId actorId) => _actorManager.Any(actorId);

        public Vector3 GetPositionOffset(IBattleSystem source, IBattleSystem target)
        {
            var offset = Vector3.zero;
            var total = _actorManager.GetUnits(stackalloc ActorId[] { ActorIds.Character, ActorIds.Monster }, _units);
            if (total == 0) return offset;

            for (var i = 0; i < TryCount; i++)
            {
                var position = source.UnitPosition + offset;
                if (!IsPositionOccupied(source.Id, position, _units)) return offset;

                if (!TryFindBattleOffset(source, target, out offset)) break;
            }

            return offset;
        }

        public bool FindTarget(ActorId actorId, ushort id, out IBattleSystem target) => _actorManager.TryGetPawn(actorId, id, out target);


        public bool TryGetChasePosition(IBattleSystem source, IBattleSystem target, out Vector3 chasePosition)
        {
            var offsetX = BattleCalculator.GetAttackOffset(source, target);
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
            if (_actorManager.GetUnits(actorId, _units) == 0) return false;

            var bestPathLen = float.PositiveInfinity;
            var sourcePosition = source.UnitPosition;
            var detectionRange = source.Stat.RateValue(StatId.DetectionRange);
            foreach (var newTarget in GetUnitsWithinRadius(_units, sourcePosition, detectionRange))
            {
                if (source.Id == newTarget.Id) continue;

                var targetPosition = newTarget.UnitPosition;
                var offsetX = BattleCalculator.GetAttackOffset(source, newTarget);
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
            if (_actorManager.GetUnits(actorId, _units) == 0) return false;

            var bestDistance = float.PositiveInfinity;
            var sourcePosition = source.UnitPosition;
            var attackRange = source.Stat.RateValue(StatId.AttackRange);
            foreach (var newTarget in GetUnitsWithinRadius(_units, sourcePosition, attackRange))
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
            var offsetX = BattleCalculator.GetAttackOffset(source, target);
            if (!TryCalculateChasePath(source.UnitPosition, target.UnitPosition, offsetX, out var newChasePosition)) return false;

            chasePosition = newChasePosition;
            return true;
        }

        private static IEnumerable<IBattleSystem> GetUnitsWithinRadius(IReadOnlyList<IBattleSystem> units, Vector3 sourcePosition, float range)
        {
            var r2 = range * range;
            foreach (var unit in units)
            {
                var unitPosition = unit.UnitPosition;
                var d2 = (unitPosition - sourcePosition).sqrMagnitude;
                if (d2 > r2) continue;

                yield return unit;
            }
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

        private static bool IsPositionOccupied(ushort id, Vector3 position, IReadOnlyList<IBattleSystem> units)
        {
            foreach (var unit in units)
            {
                if (unit.Id == id) continue;

                if (unit.State != StateTags.Battle) continue;

                if (Vector3.Distance(unit.UnitPosition, position) < 0.1f)
                {
                    return true;
                }
            }

            return false;
        }

        private bool TryFindBattleOffset(IBattleSystem source, IBattleSystem target, out Vector3 offset)
        {
            offset = Vector3.zero;
            var targetPosition = target.UnitPosition;
            for (var i = 0; i < TryCount; i++)
            {
                var sourcePosition = source.UnitPosition;
                var sourceUnitType = source.Type;
                var candidate = sourcePosition + BattleCalculator.GetPositionOffset();
                if (BattleCalculator.IsMelee(sourceUnitType))
                {
                    var zDelta = targetPosition.z - candidate.z;
                    if (Mathf.Abs(zDelta) > BattleCalculator.MinZDistance)
                    {
                        candidate.z = targetPosition.z + Mathf.Sign(-zDelta) * (BattleCalculator.MinZDistance - 0.01f);
                    }
                }

                var sourceAttackRange = source.Stat.RateValue(StatId.AttackRange);
                var targetHitRange = target.Stat.RateValue(StatId.HitRange);
                if (!BattleCalculator.CanBattle(sourceUnitType, candidate, targetPosition, sourceAttackRange, targetHitRange)) continue;

                var cell = _coordinateService.GetCellPoint(candidate);
                if (!_stageMapStore.CanMove(cell)) continue;

                offset = candidate - sourcePosition;
                return true;
            }

            return false;
        }
    }
}