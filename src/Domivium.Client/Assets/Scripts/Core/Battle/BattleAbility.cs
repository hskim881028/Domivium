using System.Collections.Generic;
using System.Linq;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.State;

namespace Domivium.Client.Core.Battle
{
    public abstract class BattleAbility
    {
        protected readonly IBattleEffectPool EffectPool;
        protected readonly IActorFinder ActorFinder;
        protected abstract IReadOnlyCollection<BattleTag> RequiredBattleTags { get; }
        protected abstract IReadOnlyCollection<BattleTag> BlockedBattleTags { get; }
        protected abstract IReadOnlyCollection<StateTag> BlockedStateTags { get; }
        public abstract BattleAbilityId Id { get; }
        public abstract float Cooldown { get; }
        public abstract BattleCueId CueId { get; }

        protected BattleAbility(IBattleEffectPool effectPool, IActorFinder actorFinder)
        {
            EffectPool = effectPool;
            ActorFinder = actorFinder;
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