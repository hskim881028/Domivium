using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Systems;
using Domivium.Client.Data.Stat;

namespace Domivium.Client.Contents.Battle.Effect
{
    public class DurabilityEffect : BattleEffect
    {
        public override BattleEffectId Id => BattleEffectIds.Durability;
        public DurabilityEffect(IItemUsageSystemCommand itemUsage, ref BattleEffectContext context) : base(itemUsage, ref context) { }

        protected override bool OnActivate()
        {
            AddGaugeModifier(StatId.Durability, -1, GaugeChannel.Add);
            return true;
        }
    }
}