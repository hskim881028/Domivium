namespace Domivium.Client.Core.Battle
{
    public readonly struct BattleAbilityContext
    {
        public IBattleSystem Source { get; private init; }
        public IBattleSystem Target { get; private init; }

        public static BattleAbilityContext Create(IBattleSystem source, IBattleSystem target)
        {
            return new BattleAbilityContext
            {
                Source = source,
                Target = target
            };
        }
    }
}