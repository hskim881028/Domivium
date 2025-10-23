using Domivium.Client.Core.Battle;
using Domivium.Client.Data.Stat;

namespace Domivium.Client.Contents.Battle.Effect
{
    public class DamageEffect : BattleEffect
    {
        public override BattleEffectId Id => BattleEffectIds.Damage;
        public override BattleCueId CueId => BattleCueIds.Damaged;

        public DamageEffect(ref BattleEffectContext context) : base(ref context) { }

        protected override bool OnActivate(BattleSystem owner)
        {
            // Context.Source.Level.CurrentValue
            var damage = BattleCalculator.GetDamage(Context.Source.Stat, owner.Stat);
            Context.Value = damage;
            AddGaugeModifier(StatId.Health, -damage, GaugeChannel.Add);
            return true;
        }
    }
}