using Domivium.Client.Core.Actors;
using UnityEngine;

namespace Domivium.Client.Core.Battle
{
    public readonly struct BattleCueContext
    {
        public ActorId ActorId { get; private init; }
        public Vector3 Position { get; private init; }
        public int Direction { get; private init; }
        public int Value { get; private init; }

        public static BattleCueContext Create(BattleAbilityContext context) => new()
        {
            ActorId = context.Source.ActorId,
            Position = context.Source.UnitPosition,
            Direction = context.Source.UnitPosition.x < context.Target.UnitPosition.x ? 1 : -1
        };

        public static BattleCueContext Create(BattleEffectContext context) => new()
        {
            Position = context.Owner.UnitPosition,
            Direction = context.Owner.UnitPosition.x < context.Source.UnitPosition.x ? 1 : -1,
            Value = context.Value
        };
    }
}