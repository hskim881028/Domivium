using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Systems;

namespace Domivium.Client.Contents.Battle.Effect
{
    public class AvoidEffect : BattleEffect
    {
        public override BattleEffectId Id => BattleEffectIds.Avoid;
        public AvoidEffect(IItemUsageSystemCommand itemUsage, ref BattleEffectContext context) : base(itemUsage, ref context) { }

        protected override bool OnActivate() => true;
    }
}