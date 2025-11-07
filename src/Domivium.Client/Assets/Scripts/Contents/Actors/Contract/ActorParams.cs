using System.Collections.Generic;
using Domivium.Client.Contents.Item;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Actors.Contract;
using Domivium.Client.Core.Battle;
using Domivium.Client.Data.Stat;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Domivium.Client.Contents.Actors.Contract
{
    public record StageFieldParams(Tilemap Tilemap) : ActorParam;

    public record CharacterParams(
        Vector2 SpawnPosition,
        UnitContext UnitContext,
        ItemContext WeaponContext,
        IReadOnlyList<BattleAbility> Abilities) : UnitParams(SpawnPosition, UnitContext, Abilities);

    public record UnitParams(
        Vector2 SpawnPosition,
        UnitContext UnitContext,
        IReadOnlyList<BattleAbility> Abilities) : PawnParams(SpawnPosition);

    public record ProjectileParams(
        ActorId Target,
        StatSet SourceStatSet,
        Vector2 SpawnPosition,
        Vector2 Direction,
        ProjectileContext ProjectileContext,
        IReadOnlyList<BattleAbility> Abilities) : PawnParams(SpawnPosition);

    public record PawnParams(Vector2 SpawnPosition) : ActorParam;

    public record PropParams(Vector2 SpawnPosition) : ActorParam;

    public record VFXParams(float DespawnTime) : ActorParam;

    public record DamageTextParams(Vector3 Position, int Damage, float DespawnTime) : VFXParams(DespawnTime);

    public record HealTextParams(Vector3 Position, int Heal, float DespawnTime) : VFXParams(DespawnTime);
}