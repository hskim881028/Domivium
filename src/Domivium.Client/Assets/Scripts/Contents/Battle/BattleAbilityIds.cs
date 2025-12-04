using Domivium.Client.Core.Battle;

namespace Domivium.Client.Contents.Battle
{
    public static class BattleAbilityIds
    {
        public static BattleAbilityId Turn = new(0);
        public static BattleAbilityId Move = new(1);
        public static BattleAbilityId Avoid = new(2);
        public static BattleAbilityId LookAt = new(3);
        public static BattleAbilityId Attack = new(4);
        public static BattleAbilityId Tracking = new(5);
        public static BattleAbilityId Reload = new(6);
        public static BattleAbilityId CancelReload = new(7);

        public static BattleAbilityId Chase = new(10);

        public static BattleAbilityId Die = new(100);
        public static BattleAbilityId Heal = new(101);
    }
}