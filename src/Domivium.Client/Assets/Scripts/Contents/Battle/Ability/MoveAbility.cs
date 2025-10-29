using Domivium.Client.Core.Battle;

namespace Domivium.Client.Contents.Battle.Ability
{
    public class MoveAbility : BattleAbility
    {
        public MoveAbility(IBattleEffectPool effectPool) : base(effectPool) { }
        public override BattleAbilityId Id => BattleAbilityIds.Move;
        public override BattleCueId CueId => BattleCueIds.Move;

        protected override bool OnActivate(ref BattleAbilityContext context)
        {
            context.Source.SetPosition(context.NextPosition);
            return true;
        }
    }
}