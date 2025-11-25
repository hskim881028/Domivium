using Domivium.Client.Core.Actors;
using UnityEngine;

namespace Domivium.Client.Core.Battle
{
    public readonly struct BattleCueContext
    {
        public ActorId ActorId { get; private init; }
        public Vector2 Position { get; private init; }
        public Vector2 Direction { get; private init; }
        public int Value { get; private init; }

        // public static BattleCueContext Create(BattleAbilityContext context) => new()
        // {
        //     ActorId = context.Source.ActorId,
        //     Position = context.Source.Position,
        //     EndPosition = context.Target.Position,
        //     Direction = context.Source.Position.x < context.Target.Position.x ? 1 : -1
        // };

        public static BattleCueContext Create(BattleEffectContext context) => new()
        {
            ActorId = context.Owner.ActorId,
            Position = context.Owner.Position,
            Value = context.Value
        };

        public static BattleCueContext Create(ActorId actorId, Vector2 position, Vector2 direction) => new()
        {
            ActorId = actorId,
            Position = position,
            Direction = direction
        };
    }
}