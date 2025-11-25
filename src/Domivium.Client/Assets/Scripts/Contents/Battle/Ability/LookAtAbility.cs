using Domivium.Client.Core.Battle;

namespace Domivium.Client.Contents.Battle.Ability
{
    public class LookAtAbility : BattleAbility
    {
        public override BattleAbilityId Id => BattleAbilityIds.LookAt;

        public LookAtAbility(IBattleEffectPool effectPool) : base(effectPool) { }

        public override float Activate(ref BattleAbilityContext context)
        {
            context.Source.SetLookAt(context.Delta);
            return 0;
        }
    }
}