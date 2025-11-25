using Domivium.Client.Core.Actors;
using UnityEngine;

namespace Domivium.Client.Core.Battle
{
    public struct BattleAbilityContext
    {
        public BattleAbilityId AbilityId { get; private init; }
        public IBattleSystem Source { get; private init; }
        public ActorId Target { get; private init; }
        public Vector2 Delta { get; private init; }
        public float DeltaTime { get; private init; }

        public static BattleAbilityContext Create(
            BattleAbilityId abilityId,
            IBattleSystem source)
            => new()
            {
                AbilityId = abilityId,
                Source = source
            };

        public static BattleAbilityContext Create(
            BattleAbilityId abilityId,
            IBattleSystem source,
            float deltaTime)
            => new()
            {
                AbilityId = abilityId,
                Source = source,
                DeltaTime = deltaTime
            };

        public static BattleAbilityContext Create(
            BattleAbilityId abilityId,
            IBattleSystem source,
            ActorId target,
            float deltaTime)
            => new()
            {
                AbilityId = abilityId,
                Source = source,
                Target = target,
                DeltaTime = deltaTime
            };

        public static BattleAbilityContext Create(
            BattleAbilityId abilityId,
            IBattleSystem source,
            Vector2 delta)
            => new()
            {
                AbilityId = abilityId,
                Source = source,
                Delta = delta
            };

        public static BattleAbilityContext Create(
            BattleAbilityId abilityId,
            IBattleSystem source,
            Vector2 delta,
            float deltaTime)
            => new()
            {
                AbilityId = abilityId,
                Source = source,
                Delta = delta,
                DeltaTime = deltaTime
            };
    }
}