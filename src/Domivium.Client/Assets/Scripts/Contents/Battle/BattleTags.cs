using Domivium.Client.Core.Battle;

namespace Domivium.Client.Contents.Battle
{
    public static class BattleTags
    {
        public static readonly BattleTag Idle = 0;
        public static readonly BattleTag Aiming = 1;
        public static readonly BattleTag Firing = 2;
        public static readonly BattleTag Reloading = 3;
        public static readonly BattleTag Avoid = 4;

        public static string ToName(this BattleTag tag)
        {
            return tag switch
            {
                _ when tag == Idle => "Idle",
                _ when tag == Aiming => "Aiming",
                _ when tag == Firing => "Firing",
                _ when tag == Reloading => "Reloading",
                _ when tag == Avoid => "Avoid",
                _ => "Unknown"
            };
        }
    }
}