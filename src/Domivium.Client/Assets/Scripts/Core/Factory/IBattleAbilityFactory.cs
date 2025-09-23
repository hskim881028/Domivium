using Domivium.Client.Core.Battle;

namespace Domivium.Client.Core.Factory
{
    public interface IBattleAbilityFactory
    {
        public BattleAbilitySpec Create(BattleAbilityId id);
    }
}