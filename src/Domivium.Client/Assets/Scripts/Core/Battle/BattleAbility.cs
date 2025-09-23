using System.Collections.Generic;
using System.Linq;
using Domivium.Client.Core.State;
using Domivium.Client.Core.Utility;

namespace Domivium.Client.Core.Battle
{
    public abstract class BattleAbility
    {
        protected readonly IBattleEffectPool EffectPool;
        protected virtual IReadOnlyCollection<BattleTag> RequiredBattleTags => TagGenerator.EmptyBattleTag;
        protected virtual IReadOnlyCollection<BattleTag> BlockedBattleTags => TagGenerator.EmptyBattleTag;
        protected virtual IReadOnlyCollection<StateTag> BlockedStateTags => TagGenerator.DefaultBlockedStateTag;
        public abstract BattleAbilityId Id { get; }
        public abstract BattleCueId CueId { get; }
        public abstract float Cooldown { get; }
        public virtual bool ApplyAttackSpeed => false;

        protected BattleAbility(IBattleEffectPool effectPool)
        {
            EffectPool = effectPool;
        }

        public bool TryActivate(ref BattleAbilityContext abilityContext) => PassesTagRequirements(abilityContext.Source) && OnActivate(ref abilityContext);

        protected abstract bool OnActivate(ref BattleAbilityContext context);

        private bool PassesTagRequirements(IBattleSystem source)
        {
            if (BlockedStateTags.Contains(source.State))
            {
                return false;
            }

            foreach (var tag in BlockedBattleTags)
            {
                if (source.Contains(tag)) return false;
            }

            foreach (var tag in RequiredBattleTags)
            {
                if (!source.Contains(tag)) return false;
            }

            return true;
        }
    }
}