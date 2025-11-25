using System.Collections.Generic;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Systems;
using Domivium.Client.Core.Utility;
using Domivium.Client.Data.Stat;

namespace Domivium.Client.Contents.Battle.Effect
{
    public class ReloadEffect : BattleEffect
    {
        public override BattleEffectId Id => BattleEffectIds.Reload;
        public override IReadOnlyCollection<BattleEffectTag> GrantedEffectTags => TagGenerator.SetBattleTag(BattleEffectTags.Reloading);

        public ReloadEffect(IItemUsageSystemCommand itemUsage, ref BattleEffectContext context) : base(itemUsage, ref context) { }

        public override void Reset(BattleEffectContext context)
        {
            base.Reset(context);
            var reloadSpeed = context.Owner.Stat.RateValue(StatId.ReloadSpeed);
            Duration = reloadSpeed;
            PeriodicInterval = reloadSpeed;
        }

        protected override bool OnActivate()
        {
            if (Context.Owner.ActorId == ActorId.Character)
            {
                var capacity = Context.Owner.Stat.Value(StatId.ProjectileCapacity);
                if (capacity == 0) return false;

                AddActionPeriodicModifier(() => Reload(capacity));
                return true;
            }

            if (Context.Owner.ActorId == ActorId.Monster)
            {
                var capacity = Context.Owner.Stat.Value(StatId.ProjectileCapacity);
                AddGaugePeriodicModifier(StatId.ProjectileCapacity, capacity, GaugeChannel.Set);
                return true;
            }

            return false;
        }

        private void Reload(int capacity)
        {
            ItemUsage.Reload(capacity);
        }
    }
}