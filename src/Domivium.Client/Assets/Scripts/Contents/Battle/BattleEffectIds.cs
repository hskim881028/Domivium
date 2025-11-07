using Domivium.Client.Core.Battle;

namespace Domivium.Client.Contents.Battle
{
    public static class BattleEffectIds
    {
        public static BattleEffectId Attack = new(0);
        public static BattleEffectId Damage = new(1);
        public static BattleEffectId Durability = new(2);
        public static BattleEffectId Reload = new(3);

        public static BattleEffectId Heal = new(999);
    }
}