using Domivium.Client.Core.Battle;

namespace Domivium.Client.Contents.Battle
{
    public static class BattleCueIds
    {
        public static BattleCueId Move = new(1);
        public static BattleCueId Attack = new(2);
        public static BattleCueId Avoid = new(3);

        public static BattleCueId Damaged = new(100);
        public static BattleCueId Heal = new(101);
        public static BattleCueId Healed = new(102);
        public static BattleCueId DropSoul = new(103);
        public static BattleCueId LevelUp = new(104);
    }
}