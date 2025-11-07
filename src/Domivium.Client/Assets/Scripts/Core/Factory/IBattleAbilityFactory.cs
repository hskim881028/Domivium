using System.Collections.Generic;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Battle;

namespace Domivium.Client.Core.Factory
{
    public interface IBattleAbilityFactory
    {
        public IReadOnlyList<BattleAbility> GetAbilities(ActorId actorId);
    }
}