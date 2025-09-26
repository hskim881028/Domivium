using Domivium.Client.Core.Battle;

namespace Domivium.Client.Contents.Battle.Ability
{
    public class HealAbility : BattleAbility
    {
        public override BattleAbilityId Id => BattleAbilityIds.Heal;
        public override BattleCueId CueId => BattleCueIds.Heal;
        public override float Cooldown => 1;
        public override bool ApplyAttackSpeed => true;

        public HealAbility(IBattleEffectPool effectPool) : base(effectPool) { }

        protected override bool OnActivate(ref BattleAbilityContext context)
        {
            var effect = EffectPool.Get(BattleEffectIds.Heal, context);
            context.Target.ActivateEffect(effect);
            return true;
        }
    }
}