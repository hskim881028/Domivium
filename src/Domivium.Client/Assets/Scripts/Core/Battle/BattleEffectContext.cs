namespace Domivium.Client.Core.Battle
{
    public struct BattleEffectContext
    {
        public BattleEffectId EffectId { get; private init; }
        public BattleAbilityId AbilityId { get; private init; }
        public IBattleSystem Source { get; private init; }
        public IBattleSystem Owner { get; private init; }
        public int Value { get; set; }

        public static BattleEffectContext Create(BattleEffectId effectId, BattleAbilityContext context) => new()
        {
            EffectId = effectId,
            AbilityId = context.AbilityId,
            Source = context.Source,
            Owner = context.Target
        };
    }
}