namespace Domivium.Client.Core.Battle
{
    public readonly struct BattleAbilityContext
    {
        public BattleAbilityId AbilityId { get; private init; }
        public IBattleSystem Source { get; private init; }
        public IBattleSystem Target { get; private init; }

        public static BattleAbilityContext Create(
            BattleAbilityId abilityId,
            IBattleSystem source,
            IBattleSystem target)
            => new()
            {
                AbilityId = abilityId,
                Source = source,
                Target = target
            };
    }
}