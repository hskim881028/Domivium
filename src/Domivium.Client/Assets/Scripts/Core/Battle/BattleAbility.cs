using System.Collections.Generic;
using System.Linq;
using Domivium.Client.Core.Actors;

namespace Domivium.Client.Core.Battle
{
    public abstract class BattleAbility
    {
        protected readonly IBattleEffectPool EffectPool;
        protected abstract IReadOnlyCollection<ActorTag> RequiredTags { get; }
        protected abstract IReadOnlyCollection<ActorTag> BlockedTags { get; }
        protected abstract IReadOnlyCollection<ActorTag> BlockedStateTags { get; }
        public abstract BattleAbilityId Id { get; }
        public abstract float Cooldown { get; }
        public abstract BattleCueId CueId { get; }

        protected BattleAbility(IBattleEffectPool effectPool)
        {
            EffectPool = effectPool;
        }

        public abstract void Activate(IReadOnlyList<BattleSystem> targets, in BattleContext context);

        public bool PassesTagRequirements(ActorTag state, IReadOnlyCollection<ActorTag> tags)
        {
            if (BlockedStateTags.Contains(state))
            {
                return false;
            }

            return RequiredTags.All(tags.Contains) && BlockedTags.All(t => !tags.Contains(t));
        }
    }
}