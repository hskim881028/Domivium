using Domivium.Client.Core.Battle;

namespace Domivium.Client.Core.Factory
{
    public interface IBattleEffectFactory
    {
        public BattleEffect Create(BattleEffectId id, ref BattleEffectContext context);
    }
}