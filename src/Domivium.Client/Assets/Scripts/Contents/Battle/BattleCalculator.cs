using Domivium.Client.Contents.Actors;
using Domivium.Client.Core.Battle;
using Domivium.Client.Data.Stat;
using UnityEngine;

namespace Domivium.Client.Contents.Battle
{
    public static class BattleCalculator
    {
        private const float MinZDistance = 0.5f;

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

        public static bool CanBattle(IBattleSystem source, IBattleSystem target)
        {
            var distance = Vector3.Distance(source.UnitPosition, target.UnitPosition);
            var range = source.Stat.RateValue(StatId.AttackRange) + target.Stat.RateValue(StatId.HitRange);
            if (source.Type == UnitTypes.Ranged || source.Type == UnitTypes.Support)
            {
                return distance < range;
            }

            var zDistance = Mathf.Abs(source.UnitPosition.z - target.UnitPosition.z);
            if (zDistance > MinZDistance) return false;

            return distance < range;
        }
    }
}