namespace Domivium.Client.Core.Battle
{
    public struct BattleEffectContext
    {
        public IBattleSystem Source { get; private init; }
        public IBattleSystem Owner { get; private init; }
        public BattleAbility Ability { get; init; }
        public int Damage { get; set; }

        public static BattleEffectContext Create(BattleAbilityContext context, BattleAbility ability) => new()
        {
            Source = context.Source,
            Owner = context.Target,
            Ability = ability
        };
    }
}