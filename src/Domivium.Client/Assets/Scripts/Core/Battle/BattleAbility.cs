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
        public virtual BattleCueId CueId => BattleCueId.None;
        public float Cooldown { get; private set; }

        protected BattleAbility(IBattleEffectPool effectPool)
        {
            EffectPool = effectPool;
        }

        public void SetCooldown(float cooldown)
        {
            Cooldown = cooldown;
        }

        public virtual bool CanActivate(IBattleSystem source)
        {
            if (BlockedStateTags.Contains(source.State))
            {
                return false;
            }

            foreach (var tag in BlockedBattleTags)
            {
                if (source.ContainsTag(tag)) return false;
            }

            foreach (var tag in RequiredBattleTags)
            {
                if (!source.ContainsTag(tag)) return false;
            }

            return true;
        }

        public bool TryActivate(ref BattleAbilityContext abilityContext) => OnActivate(ref abilityContext);

        protected abstract bool OnActivate(ref BattleAbilityContext context);
    }
}