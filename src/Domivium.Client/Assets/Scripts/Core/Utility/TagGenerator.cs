using System.Collections.Generic;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.State;

namespace Domivium.Client.Core.Utility
{
    public static class TagGenerator
    {
        private static readonly HashSet<BattleTag> EmptyBattle = new();
        private static readonly HashSet<StateTag> EmptyState = new();
        private static readonly HashSet<StateTag> DefaultBlockedState = new() { StateTag.Die, StateTag.Despawn, StateTag.Terminated };
        
        public static IReadOnlyCollection<BattleTag> EmptyBattleTag => EmptyBattle;
        public static HashSet<BattleTag> SetBattleTag(params BattleTag[] values) => new(values);
        
        public static IReadOnlyCollection<StateTag> EmptyStateTag => EmptyState;
        public static IReadOnlyCollection<StateTag> DefaultBlockedStateTag => DefaultBlockedState;
        public static HashSet<StateTag> SetStateTag(params StateTag[] values) => new(values);
    }
}