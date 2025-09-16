namespace Domivium.Client.Core.Battle
{
    public interface IBattleAbilityFactory
    {
        public BattleAbilitySpec Create(BattleAbilityId id);
    }
}