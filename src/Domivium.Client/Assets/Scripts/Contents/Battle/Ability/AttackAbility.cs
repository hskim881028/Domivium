using Domivium.Client.Core.Battle;

namespace Domivium.Client.Contents.Battle.Ability
{
    public class AttackAbility : BattleAbility
    {
        public override BattleAbilityId Id => BattleAbilityIds.Attack;
        public override BattleCueId CueId => BattleCueIds.Attack;
        public override float Cooldown => 1;
        public override bool ApplyAttackSpeed => true;

        public AttackAbility(IBattleEffectPool effectPool) : base(effectPool) { }

        protected override bool OnActivate(ref BattleAbilityContext context)
        {
            var effect = EffectPool.Get(BattleEffectIds.Damage, context);
            context.Target.ActivateEffect(effect);
            return true;
        }
    }
}