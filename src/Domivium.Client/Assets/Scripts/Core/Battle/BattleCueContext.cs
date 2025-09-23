using UnityEngine;

namespace Domivium.Client.Core.Battle
{
    public readonly struct BattleCueContext
    {
        public Vector3 Position { get; private init; }
        public int Value { get; private init; }

        public static BattleCueContext Create(BattleAbilityContext context) => new()
        {
            Position = context.Source.UnitPosition
        };

        public static BattleCueContext Create(BattleEffectContext context) => new()
        {
            Position = context.Owner.UnitPosition,
            Value = context.Value
        };
    }
}