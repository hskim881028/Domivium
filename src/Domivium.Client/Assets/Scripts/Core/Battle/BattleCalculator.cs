using Domivium.Client.Data.Stat;

namespace Domivium.Client.Core.Battle
{
    public static class BattleCalculator
    {
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
    }
}