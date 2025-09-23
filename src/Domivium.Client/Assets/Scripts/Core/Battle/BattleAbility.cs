using System.Collections.Generic;
using System.Linq;
using Domivium.Client.Core.State;
using NUnit.Framework.Constraints;

namespace Domivium.Client.Core.Battle
{
    public abstract class BattleAbility
    {
        protected readonly IBattleEffectPool EffectPool;
        protected abstract IReadOnlyCollection<BattleTag> RequiredBattleTags { get; }
        protected abstract IReadOnlyCollection<BattleTag> BlockedBattleTags { get; }
        protected abstract IReadOnlyCollection<StateTag> BlockedStateTags { get; }
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