using Domivium.Client.Core.Battle;
using Domivium.Client.Data.Stat;

namespace Domivium.Client.Contents.Battle.Effect
{
    public class LevelUpEffect : BattleEffect
    {
        public override BattleEffectId Id => BattleEffectIds.LevelUp;
        public override BattleCueId CueId => BattleCueIds.LevelUp;

        public LevelUpEffect(ref BattleEffectContext context) : base(ref context) { }

        protected override bool OnActivate(BattleSystem owner)
        {
            AddStatModifier(StatId.Level, 1, StatChannel.Add);
            AddStatModifier(StatId.Health, 20, StatChannel.AddMultiplier);
            return true;
        }
    }
}