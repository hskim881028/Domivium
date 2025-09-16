using System.Collections.Generic;
using Domivium.Client.Core.Battle;
using Domivium.Client.Data.Stat;

namespace Domivium.Client.Contents.Battle.Effect
{
    public class DamageEffect : BattleEffect
    {
        protected override IReadOnlyCollection<BattleTag> RequiredTags => BattleTag.Empty;
        protected override IReadOnlyCollection<BattleTag> BlockedTags => BattleTag.OnlyDie;
        public override IReadOnlyCollection<BattleTag> GrantedTags => BattleTag.Empty;
        public override BattleEffectId Id => BattleEffectIds.Damage;
        public override float Duration => 0;
        public override float PeriodicInterval => 0;
        public override BattleCueId CueId => BattleCueIds.Damaged;
        public override BattleCueId PeriodicCueId => BattleCueId.None;
        public override BattleCueId DeactivateCueId => BattleCueId.None;

        public DamageEffect(BattleContext context) : base(context) { }

        public override void Activate(BattleSystem owner)
        {
            var damage = BattleCalculator.GetDamage(Context.Source.Stat, owner.Stat);
            AddGaugeModifier(StatId.Health, -damage, GaugeChannel.Add);
        }
    }
}