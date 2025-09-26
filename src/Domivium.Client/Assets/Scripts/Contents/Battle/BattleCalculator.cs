using Domivium.Client.Contents.Actors;
using Domivium.Client.Core.Actors.Unit;
using Domivium.Client.Core.Battle;
using Domivium.Client.Data.Stat;
using UnityEngine;

namespace Domivium.Client.Contents.Battle
{
    public static class BattleCalculator
    {
        public const float MinZDistance = 0.5f;
        private const float AttackOffset = 0.1f;

        public static bool IsMelee(UnitType unitType) => unitType == UnitTypes.Melee || unitType == UnitTypes.Tank;

        public static int GetDamage(StatSet attacker, StatSet defender)
        {
            var atk = attacker.Value(StatId.Attack);
            var def = defender.Value(StatId.Defense);
            var damage = atk - def;
            if (damage < 0)
            {
                damage = 0;
            }
            return damage;
        }

        public static Vector3 GetPositionOffset()
        {
            var random = Random.insideUnitCircle * AttackOffset;
            return new Vector3(random.x, 0, random.y);
        }

        public static float GetAttackOffset(IBattleSystem source, IBattleSystem target)
        {
            if (!IsMelee(source.Type)) return 0;

            return source.Stat.RateValue(StatId.AttackRange) + target.Stat.RateValue(StatId.HitRange) - AttackOffset;
        }

        public static bool CanBattle(IBattleSystem source, IBattleSystem target)
        {
            var sourceUnitType = source.Type;
            var sourcePosition = source.UnitPosition;
            var targetPosition = target.UnitPosition;
            var sourceAttackRange = source.Stat.RateValue(StatId.AttackRange);
            var targetHitRange = target.Stat.RateValue(StatId.HitRange);
            return CanBattle(sourceUnitType, sourcePosition, targetPosition, sourceAttackRange, targetHitRange);
        }

        public static bool CanBattle(
            UnitType sourceUnitType,
            Vector3 sourcePosition,
            Vector3 targetPosition,
            float sourceAttackRange,
            float targetHitRange)
        {
            var distance = Vector3.Distance(sourcePosition, targetPosition);
            var range = sourceAttackRange + targetHitRange;
            if (sourceUnitType == UnitTypes.Ranged || sourceUnitType == UnitTypes.Support)
            {
                return distance < range;
            }

            var zDistance = Mathf.Abs(sourcePosition.z - targetPosition.z);
            if (zDistance > MinZDistance) return false;

            return distance < range;
        }
    }
}