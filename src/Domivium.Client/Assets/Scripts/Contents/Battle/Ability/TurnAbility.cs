using Domivium.Client.Core.Battle;

namespace Domivium.Client.Contents.Battle.Ability
{
    public class TurnAbility : BattleAbility
    {
        public override BattleAbilityId Id => BattleAbilityIds.Turn;

        public TurnAbility(IBattleEffectPool effectPool) : base(effectPool) { }

        protected override bool OnActivate(ref BattleAbilityContext context)
        {
            context.Delta.Normalize();
            context.Source.SetDirection(context.Delta);
            return true;
        }
    }
}