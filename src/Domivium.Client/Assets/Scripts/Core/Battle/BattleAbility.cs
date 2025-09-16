using System.Collections.Generic;
using System.Linq;

namespace Domivium.Client.Core.Battle
{
    public abstract class BattleAbility
    {
        protected readonly IBattleEffectPool EffectPool;
        protected abstract IReadOnlyCollection<BattleTag> RequiredTags { get; }
        protected abstract IReadOnlyCollection<BattleTag> BlockedTags { get; }
        public abstract BattleAbilityId Id { get; }
        public abstract float Cooldown { get; }
        public abstract BattleCueId CueId { get; }

        protected BattleAbility(IBattleEffectPool effectPool)
        {
            EffectPool = effectPool;
        }

        public abstract void Activate(IReadOnlyList<BattleSystem> targets, in BattleContext context);

        public bool PassesTagRequirements(IReadOnlyCollection<BattleTag> tags)
            => RequiredTags.All(tags.Contains) && BlockedTags.All(t => !tags.Contains(t));
    }
}