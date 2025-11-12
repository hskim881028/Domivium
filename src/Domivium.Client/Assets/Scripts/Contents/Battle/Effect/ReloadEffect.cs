using System.Collections.Generic;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Utility;
using Domivium.Client.Data.Stat;

namespace Domivium.Client.Contents.Battle.Effect
{
    public class ReloadEffect : BattleEffect
    {
        public override BattleEffectId Id => BattleEffectIds.Reload;
        public override IReadOnlyCollection<BattleEffectTag> GrantedEffectTags => TagGenerator.SetBattleTag(BattleEffectTags.Reloading);

        public ReloadEffect(ref BattleEffectContext context) : base(ref context)
        {
            var reloadSpeed = context.Owner.Stat.RateValue(StatId.ReloadSpeed);
            Duration = reloadSpeed;
            PeriodicInterval = reloadSpeed;
        }

        protected override bool OnActivate()
        {
            var projectileCapacity = Context.Owner.Stat.Value(StatId.ProjectileCapacity);
            AddGaugePeriodicModifier(StatId.ProjectileCapacity, projectileCapacity, GaugeChannel.Max);
            return true;
        }
    }
}