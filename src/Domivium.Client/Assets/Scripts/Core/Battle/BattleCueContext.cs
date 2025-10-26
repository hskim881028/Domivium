using Domivium.Client.Core.Actors;
using UnityEngine;

namespace Domivium.Client.Core.Battle
{
    public readonly struct BattleCueContext
    {
        public ActorId ActorId { get; private init; }
        public PawnType PawnType { get; private init; }
        public Vector3 Position { get; private init; }
        public Vector3 EndPosition { get; private init; }
        public int Direction { get; private init; }
        public int Value { get; private init; }

        public static BattleCueContext Create(BattleAbilityContext context) => new()
        {
            ActorId = context.Source.ActorId,
            PawnType = context.Source.Type,
            Position = context.Source.UnitPosition,
            EndPosition = context.Target.UnitPosition,
            Direction = context.Source.UnitPosition.x < context.Target.UnitPosition.x ? 1 : -1
        };

        public static BattleCueContext Create(BattleEffectContext context) => new()
        {
            ActorId = context.Owner.ActorId,
            PawnType = context.Owner.Type,
            Position = context.Owner.UnitPosition,
            EndPosition = context.Source.UnitPosition,
            Direction = context.Owner.UnitPosition.x < context.Source.UnitPosition.x ? 1 : -1,
            Value = context.Value
        };

        public static BattleCueContext Create(ActorId actorId, Vector3 position, Vector3 endPosition) => new()
        {
            ActorId = actorId,
            Position = position,
            EndPosition = endPosition
        };
    }
}