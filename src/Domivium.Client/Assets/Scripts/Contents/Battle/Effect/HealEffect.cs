using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Systems;
using Domivium.Client.Data.Stat;

namespace Domivium.Client.Contents.Battle.Effect
{
    public class HealEffect : BattleEffect
    {
        public override BattleEffectId Id => BattleEffectIds.Heal;
        public override BattleCueId CueId => BattleCueIds.Healed;

        public HealEffect(IItemUsageSystemCommand itemUsage, ref BattleEffectContext context) : base(itemUsage, ref context) { }

        protected override bool OnActivate()
        {
            var heal = Context.Owner.Stat.Value(StatId.Attack);
            Context.Value = heal;
            AddGaugeModifier(StatId.Health, heal, GaugeChannel.Add);
            return true;
        }
    }
}