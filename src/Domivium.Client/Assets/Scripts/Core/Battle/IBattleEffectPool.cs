namespace Domivium.Client.Core.Battle
{
    public interface IBattleEffectPool
    {
        public BattleEffectSpec Get(BattleEffectId id, IBattleSystem source, IBattleSystem owner);
    }
}