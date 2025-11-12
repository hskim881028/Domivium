using System.Collections.Generic;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.State;

namespace Domivium.Client.Core.Utility
{
    public static class TagGenerator
    {
        private static readonly HashSet<BattleEffectTag> EmptyBattleEffect = new();
        private static readonly HashSet<StateTag> EmptyState = new();
        private static readonly HashSet<StateTag> DefaultBlockedState = new() { StateTag.Die, StateTag.Despawn, StateTag.Terminated };

        public static IReadOnlyCollection<BattleEffectTag> EmptyBattleEffectTag => EmptyBattleEffect;
        public static HashSet<BattleEffectTag> SetBattleTag(params BattleEffectTag[] values) => new(values);

        public static IReadOnlyCollection<StateTag> EmptyStateTag => EmptyState;
        public static IReadOnlyCollection<StateTag> DefaultBlockedStateTag => DefaultBlockedState;
        public static HashSet<StateTag> SetStateTag(params StateTag[] values) => new(values);
    }
}