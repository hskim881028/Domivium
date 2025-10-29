using Domivium.Client.Core.Battle;

namespace Domivium.Client.Contents.Battle.Ability
{
    public class AvoidAbility : BattleAbility
    {
        public override BattleAbilityId Id => BattleAbilityIds.Avoid;
        public override BattleCueId CueId => BattleCueIds.Avoid;
        
        public override float Cooldown => 1;

        public AvoidAbility(IBattleEffectPool effectPool) : base(effectPool) { }

        protected override bool OnActivate(ref BattleAbilityContext context)
        {
            context.Source.SetPosition(context.NextPosition);
            return true;
        }
    }
}