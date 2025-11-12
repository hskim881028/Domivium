using System.Collections.Generic;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Utility;

namespace Domivium.Client.Contents.Battle.Ability
{
    public class CancelReloadAbility : BattleAbility
    {
        protected override IReadOnlyCollection<BattleEffectTag> RequiredEffectTags => TagGenerator.SetBattleTag(BattleEffectTags.Reloading);
        public override BattleAbilityId Id => BattleAbilityIds.CancelReload;

        public CancelReloadAbility(IBattleEffectPool effectPool) : base(effectPool) { }

        public override float Activate(ref BattleAbilityContext context)
        {
            context.Source.DeactivateEffect(BattleEffectIds.Reload);
            return 0;
        }
    }
}