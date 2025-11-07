using System.Collections.Generic;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Utility;

namespace Domivium.Client.Contents.Battle.Ability
{
    public class CancelReloadAbility : BattleAbility
    {
        protected override IReadOnlyCollection<BattleTag> RequiredBattleTags => TagGenerator.SetBattleTag(BattleTags.Reloading);
        public override BattleAbilityId Id => BattleAbilityIds.CancelReload;

        public CancelReloadAbility(IBattleEffectPool effectPool) : base(effectPool) { }

        protected override bool OnActivate(ref BattleAbilityContext context)
        {
            context.Source.DeactivateEffect(BattleEffectIds.Reload);
            return true;
        }
    }
}