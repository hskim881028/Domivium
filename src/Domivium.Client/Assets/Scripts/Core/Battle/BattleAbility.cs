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

        public bool TryActivate(BattleSystem source, ref BattleContext context)
        {
            return PassesTagRequirements(source.TagSet) && OnActivate(source, ref context);
        }

        protected abstract bool OnActivate(BattleSystem source, ref BattleContext context);

        private bool PassesTagRequirements(TagSet tagSet)
        {
            if (BlockedStateTags.Contains(tagSet.State.CurrentValue))
            {
                return false;
            }

            foreach (var tag in BlockedTags)
            {
                if (tagSet.Contains(tag)) return false;
            }

            foreach (var tag in RequiredTags)
            {
                if (!tagSet.Contains(tag)) return false;
            }

            return true;
        }
    }
}