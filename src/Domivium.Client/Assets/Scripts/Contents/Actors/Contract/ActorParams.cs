using System.Collections.Generic;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Actors.Contract;
using Domivium.Client.Core.Battle;
using Domivium.Client.Data.Context;
using Domivium.Client.Data.Stat;
using UnityEngine;

namespace Domivium.Client.Contents.Actors.Contract
{
    public record PawnParams(Vector2 SpawnPosition) : ActorParam;

    public record ProjectileParams(
        int Id,
        StatSet SourceStatSet,
        ActorId SourceActorId,
        ActorId TargetActorId,
        Vector2 SpawnPosition,
        Vector2 Direction,
        IReadOnlyList<BattleAbility> Abilities) : PawnParams(SpawnPosition);

    public record UnitParams(
        Vector2 SpawnPosition,
        UnitContext UnitContext,
        IReadOnlyList<BattleAbility> Abilities) : PawnParams(SpawnPosition);

    public record LobbyCharacterParams(
        Vector2 SpawnPosition,
        UnitContext UnitContext,
        IReadOnlyList<BattleAbility> Abilities) : CharacterParams(SpawnPosition, UnitContext, Abilities);

    public record CharacterParams(
        Vector2 SpawnPosition,
        UnitContext UnitContext,
        IReadOnlyList<BattleAbility> Abilities) : UnitParams(SpawnPosition, UnitContext, Abilities);

    public record MonsterParams(
        Vector2 SpawnPosition,
        UnitContext UnitContext,
        IReadOnlyList<BattleAbility> Abilities,
        IBattleSystem Target) : UnitParams(SpawnPosition, UnitContext, Abilities);


    public record PropParams(Vector2 SpawnPosition) : ActorParam;

    public record VFXParams(float DespawnTime) : ActorParam;

    public record DamageTextParams(Vector3 Position, int Damage, float DespawnTime) : VFXParams(DespawnTime);

    public record HealTextParams(Vector3 Position, int Heal, float DespawnTime) : VFXParams(DespawnTime);
}