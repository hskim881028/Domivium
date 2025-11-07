using Domivium.Client.Core.Battle;
using Domivium.Client.Data.Stat;

namespace Domivium.Client.Contents.Battle.Effect
{
    public class AttackEffect : BattleEffect
    {
        public override BattleEffectId Id => BattleEffectIds.Attack;
        
        public AttackEffect(ref BattleEffectContext context) : base(ref context) { }

        protected override bool OnActivate()
        {
            AddGaugeModifier(StatId.ProjectileCapacity, -1, GaugeChannel.Add);
            return true;
        }
    }
}