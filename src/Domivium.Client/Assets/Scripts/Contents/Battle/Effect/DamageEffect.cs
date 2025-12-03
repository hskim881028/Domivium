using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Container;
using Domivium.Client.Data.Stat;

namespace Domivium.Client.Contents.Battle.Effect
{
    public class DamageEffect : BattleEffect
    {
        public override BattleEffectId Id => BattleEffectIds.Damage;
        public override BattleCueId CueId => BattleCueIds.Damaged;

        public DamageEffect(IUserContainer userContainer, ref BattleEffectContext context) : base(userContainer, ref context) { }

        protected override bool OnActivate()
        {
            var damage = BattleCalculator.GetDamage(Context.Source.Stat, Context.Owner.Stat);
            Context.Value = damage;
            AddGaugeModifier(StatId.Health, -damage, GaugeChannel.Add);
            return true;
        }
    }
}