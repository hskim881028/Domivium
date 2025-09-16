using System.Collections.Generic;
using Domivium.Client.Core.Battle;

namespace Domivium.Client.Contents.Battle.Ability
{
    public class SlashAbility : BattleAbility
    {
        protected override IReadOnlyCollection<BattleTag> RequiredTags => BattleTag.Empty;
        protected override IReadOnlyCollection<BattleTag> BlockedTags => BattleTag.OnlyDie;
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