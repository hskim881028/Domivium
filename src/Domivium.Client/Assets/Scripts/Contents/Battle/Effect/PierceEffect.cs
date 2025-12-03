using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Container;
using Domivium.Client.Data.Stat;

namespace Domivium.Client.Contents.Battle.Effect
{
    public class PierceEffect : BattleEffect
    {
        public override BattleEffectId Id => BattleEffectIds.Pierce;
        public PierceEffect(IUserContainer userContainer, ref BattleEffectContext context) : base(userContainer, ref context) { }

        protected override bool OnActivate()
        {
            AddGaugeModifier(StatId.Penetration, -1, GaugeChannel.Add);
            return true;
        }
    }
}