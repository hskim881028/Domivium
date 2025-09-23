using System.Collections.Generic;
using Domivium.Client.Contents.State;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.State;

namespace Domivium.Client.Contents.Battle.Ability
{
    public class SlashAbility : BattleAbility
    {
        protected override IReadOnlyCollection<BattleTag> RequiredBattleTags => BattleTags.Empty;
        protected override IReadOnlyCollection<BattleTag> BlockedBattleTags => BattleTags.Empty;
        protected override IReadOnlyCollection<StateTag> BlockedStateTags => StateTags.DefaultBlockedTag;
        public override BattleAbilityId Id => BattleAbilityIds.Slash;
        public override BattleCueId CueId => BattleCueIds.Slash;
        public override float Cooldown => 1;
        public override bool ApplyAttackSpeed => true;

        public SlashAbility(IBattleEffectPool effectPool) : base(effectPool) { }

        protected override bool OnActivate(ref BattleAbilityContext context)
        {
            var effect = EffectPool.Get(BattleEffectIds.Damage, context, this);
            context.Target.ActivateEffect(effect);
            return true;
        }
    }
}