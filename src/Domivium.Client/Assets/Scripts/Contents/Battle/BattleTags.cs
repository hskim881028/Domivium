using Domivium.Client.Core.Battle;

namespace Domivium.Client.Contents.Battle
{
    public static class BattleTags
    {
        public static readonly BattleTag Idle = 1;
        public static readonly BattleTag Aiming = 2;
        public static readonly BattleTag Firing = 3;
        public static readonly BattleTag Reloading = 4;
        public static readonly BattleTag Returning = 5;

        public static string ToName(this BattleTag tag)
        {
            return tag switch
            {
                _ when tag == Idle => "Idle",
                _ when tag == Aiming => "Aiming",
                _ when tag == Firing => "Firing",
                _ when tag == Reloading => "Reloading",
                _ when tag == Returning => "Returning",
                _ => "Unknown"
            };
        }
    }
}