using System.Collections.Generic;
using Domivium.Client.Core.Battle;

namespace Domivium.Client.Contents.Battle
{
    public static class BattleTags
    {
        private static readonly HashSet<BattleTag> EmptyInternal = new();
        public static IReadOnlyCollection<BattleTag> Empty => EmptyInternal;
    }

    public static class ActionTag
    {
        public static readonly BattleTag MeleeAttack = 0;
        public static readonly BattleTag RangedAttack = 1;
        public static readonly BattleTag Damaged = 2;
    }

    public static class BuffTag { }

    public static class DebuffTag { }
}