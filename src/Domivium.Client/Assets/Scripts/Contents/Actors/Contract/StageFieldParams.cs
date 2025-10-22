using System.Collections.Generic;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Actors.Contract;
using Domivium.Client.Core.Battle;
using UnityEngine;

namespace Domivium.Client.Contents.Actors.Contract
{
    public record StageFieldParams(BoundsInt CellBounds) : ActorParam;

    public record UnitParams(
        Vector3Int SpawnPoint,
        PawnContext PawnContext,
        IReadOnlyList<BattleAbility> Abilities) : ActorParam;

    public record CampParams(int Index, Vector3Int SpawnPoint) : ActorParam;

    public record VFXParams(float DespawnTime) : ActorParam;

    public record DamageTextParams(Vector3 Position, int Damage, float DespawnTime) : VFXParams(DespawnTime);

    public record HealTextParams(Vector3 Position, int Damage, float DespawnTime) : VFXParams(DespawnTime);

    public record SlashEffectParams(
        ActorId ActorId,
        Vector3 Position,
        int Direction,
        float DespawnTime) : VFXParams(DespawnTime);
    
    public record SoulEffectParams(Vector3 Position, Vector3 EndPosition, float DespawnTime) : VFXParams(DespawnTime);
}