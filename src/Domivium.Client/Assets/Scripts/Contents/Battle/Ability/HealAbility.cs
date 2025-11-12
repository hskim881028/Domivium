using Domivium.Client.Core.Battle;

namespace Domivium.Client.Contents.Battle.Ability
{
    public class HealAbility : BattleAbility
    {
        public override BattleAbilityId Id => BattleAbilityIds.Heal;
        public override BattleCueId CueId => BattleCueIds.Heal;

        public HealAbility(IBattleEffectPool effectPool) : base(effectPool) { }

        public override float Activate(ref BattleAbilityContext context)
        {
            // var effect = EffectPool.Get(BattleEffectIds.Heal, context.Source, context.Target);
            // context.Target.ActivateEffect(effect);
            return 0;
        }
    }
}