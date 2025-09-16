using System.Collections.Generic;
using UnitGenerator;

namespace Domivium.Client.Core.Battle
{
    [UnitOf(typeof(int), UnitGenerateOptions.ImplicitOperator | UnitGenerateOptions.Comparable)]
    public readonly partial struct BattleTag
    {
        public static readonly BattleTag None = 0;
        public static readonly BattleTag Idle = 1;
        public static readonly BattleTag Hit = 2;
        public static readonly BattleTag Die = 3;

        private static readonly HashSet<BattleTag> EmptyInternal = new();
        private static readonly HashSet<BattleTag> OnlyDieInternal = new() { Die };
        public static IReadOnlyCollection<BattleTag> Empty => EmptyInternal;
        public static IReadOnlyCollection<BattleTag> OnlyDie => OnlyDieInternal;
        public static HashSet<BattleTag> Set(params BattleTag[] values) => new(values);
    }
}