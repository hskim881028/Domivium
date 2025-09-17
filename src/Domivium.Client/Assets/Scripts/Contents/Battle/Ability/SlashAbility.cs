using System.Collections.Generic;
using Domivium.Client.Contents.Actors;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Battle;

namespace Domivium.Client.Contents.Battle.Ability
{
    public class SlashAbility : BattleAbility
    {
        protected override IReadOnlyCollection<ActorTag> RequiredTags => ActorTags.Empty;
        protected override IReadOnlyCollection<ActorTag> BlockedTags => ActorTags.Empty;
        protected override IReadOnlyCollection<ActorTag> BlockedStateTags => ActorTags.DefaultBlockedTag;
        public override BattleAbilityId Id => BattleAbilityIds.Slash;
        public override float Cooldown => 0;
        public override BattleCueId CueId => BattleCueIds.Slash;

        public SlashAbility(IBattleEffectPool effectPool) : base(effectPool) { }

        public override void Activate(IReadOnlyList<BattleSystem> targets, in BattleContext context)
        {
            foreach (var target in targets)
            {
                var effect = EffectPool.Get(BattleEffectIds.Damage, context);
                target.ActivateEffect(effect);
            }
        }
    }
}