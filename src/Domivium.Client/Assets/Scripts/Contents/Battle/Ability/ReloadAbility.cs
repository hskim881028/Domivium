using System.Collections.Generic;
using Domivium.Client.Core.Actors;
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

            if (source.ActorId == ActorId.Monster) return true;

            var max = source.Gauge.Max(StatId.ProjectileCapacity);
            if (max == 0) return false;

            var cur = source.Gauge.Current(StatId.ProjectileCapacity);
            var capacity = source.Stat.Value(StatId.ProjectileCapacity);
            if (cur == capacity) return false;

            return cur < capacity;
        }

        public override float Activate(ref BattleAbilityContext context)
        {
            var effect = EffectPool.Get(BattleEffectIds.Reload, context.Source, context.Source);
            context.Source.ActivateEffect(effect);
            return 0;
        }
    }
}