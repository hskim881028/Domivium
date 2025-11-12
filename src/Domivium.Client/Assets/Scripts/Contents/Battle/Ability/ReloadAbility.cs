using System.Collections.Generic;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Utility;
using Domivium.Client.Data.Stat;

namespace Domivium.Client.Contents.Battle.Ability
{
    public class ReloadAbility : BattleAbility
    {
        public override BattleAbilityId Id => BattleAbilityIds.Reload;
        protected override IReadOnlyCollection<BattleEffectTag> BlockedEffectTags => TagGenerator.SetBattleTag(BattleEffectTags.Reloading);

        public ReloadAbility(IBattleEffectPool effectPool) : base(effectPool) { }

        public override bool CanActivate(IBattleSystem source)
        {
            if (!base.CanActivate(source)) return false;

            var cur = source.Gauge.Current(StatId.ProjectileCapacity);
            var max = source.Gauge.Max(StatId.ProjectileCapacity);
            return cur < max;
        }

        public override float Activate(ref BattleAbilityContext context)
        {
            var effect = EffectPool.Get(BattleEffectIds.Reload, context.Source, context.Source);
            context.Source.ActivateEffect(effect);
            return 0;
        }
    }
}