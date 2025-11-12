using System.Collections.Generic;
using System.Linq;
using Domivium.Client.Core.State;
using Domivium.Client.Core.Utility;

namespace Domivium.Client.Core.Battle
{
    public abstract class BattleAbility
    {
        protected readonly IBattleEffectPool EffectPool;

        public abstract BattleAbilityId Id { get; }
        protected virtual IReadOnlyCollection<BattleEffectTag> RequiredEffectTags => TagGenerator.EmptyBattleEffectTag;
        protected virtual IReadOnlyCollection<BattleEffectTag> BlockedEffectTags => TagGenerator.EmptyBattleEffectTag;
        protected virtual IReadOnlyCollection<StateTag> BlockedStateTags => TagGenerator.DefaultBlockedStateTag;
        public virtual BattleCueId CueId => BattleCueId.None;

        protected BattleAbility(IBattleEffectPool effectPool)
        {
            EffectPool = effectPool;
        }

        public virtual bool CanActivate(IBattleSystem source)
        {
            if (BlockedStateTags.Contains(source.State))
            {
                return false;
            }

            foreach (var tag in BlockedEffectTags)
            {
                if (source.ContainsEffectTag(tag)) return false;
            }

            foreach (var tag in RequiredEffectTags)
            {
                if (!source.ContainsEffectTag(tag)) return false;
            }

            return true;
        }

        public abstract float Activate(ref BattleAbilityContext context);
    }
}