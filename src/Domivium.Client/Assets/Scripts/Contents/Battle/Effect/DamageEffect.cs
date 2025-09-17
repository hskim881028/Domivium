using System.Collections.Generic;
using Domivium.Client.Contents.Actors;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Battle;
using Domivium.Client.Data.Stat;

namespace Domivium.Client.Contents.Battle.Effect
{
    public class DamageEffect : BattleEffect
    {
        protected override IReadOnlyCollection<ActorTag> RequiredTags => ActorTags.Empty;
        protected override IReadOnlyCollection<ActorTag> BlockedTags => ActorTags.Empty;
        protected override IReadOnlyCollection<ActorTag> BlockedStateTags => ActorTags.DefaultBlockedTag;
        public override IReadOnlyCollection<ActorTag> GrantedTags => ActorTags.Empty;
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
            Context.Damage = damage;
            AddGaugeModifier(StatId.Health, -damage, GaugeChannel.Add);
        }
    }
}