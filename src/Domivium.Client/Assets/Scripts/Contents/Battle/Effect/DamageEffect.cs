using System.Collections.Generic;
using Domivium.Client.Contents.State;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.State;
using Domivium.Client.Data.Stat;

namespace Domivium.Client.Contents.Battle.Effect
{
    public class DamageEffect : BattleEffect
    {
        protected override IReadOnlyCollection<BattleTag> RequiredBattleTags => BattleTags.Empty;
        protected override IReadOnlyCollection<BattleTag> BlockedBattleTags => BattleTags.Empty;
        protected override IReadOnlyCollection<StateTag> BlockedStateTags => StateTags.DefaultBlockedTag;
        public override IReadOnlyCollection<BattleTag> GrantedBattleTags => BattleTags.Empty;
        public override BattleEffectId Id => BattleEffectIds.Damage;
        public override float Duration => 0;
        public override float PeriodicInterval => 0;
        public override BattleCueId CueId => BattleCueIds.Damaged;
        public override BattleCueId PeriodicCueId => BattleCueId.None;
        public override BattleCueId DeactivateCueId => BattleCueId.None;

        public DamageEffect(ref BattleEffectContext context) : base(ref context) { }

        protected override bool OnActivate(BattleSystem owner)
        {
            var damage = BattleCalculator.GetDamage(Context.Source.Stat, owner.Stat);
            Context.Damage = damage;
            AddGaugeModifier(StatId.Health, -damage, GaugeChannel.Add);
            return true;
        }
    }
}