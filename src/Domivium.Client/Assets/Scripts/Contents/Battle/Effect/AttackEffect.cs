using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Systems;
using Domivium.Client.Data.Stat;

namespace Domivium.Client.Contents.Battle.Effect
{
    public class AttackEffect : BattleEffect
    {
        public override BattleEffectId Id => BattleEffectIds.Attack;

        public AttackEffect(IItemUsageSystemCommand itemUsage, ref BattleEffectContext context) : base(itemUsage, ref context) { }

        protected override bool OnActivate()
        {
            if (Context.Owner.ActorId == ActorId.Character)
            {
                return ItemUsage.UseProjectile();
            }

            if (Context.Owner.ActorId == ActorId.Monster)
            {
                AddGaugeModifier(StatId.ProjectileCapacity, -1, GaugeChannel.Add);
                return true;
            }

            return false;
        }
    }
}