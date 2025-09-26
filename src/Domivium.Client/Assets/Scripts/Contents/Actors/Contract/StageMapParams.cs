using System.Collections.Generic;
using Domivium.Client.Core.Actors.Contract;
using Domivium.Client.Core.Battle;
using UnityEngine;

namespace Domivium.Client.Contents.Actors.Contract
{
    public record StageMapParams(BoundsInt CellBounds) : ActorParam;

    public record UnitParams(
        Vector3Int SpawnPoint,
        UnitContext UnitContext,
        IReadOnlyList<BattleAbility> Abilities) : ActorParam;

    public record CampParams(
        int Index,
        Vector3Int SpawnPoint,
        Vector3 VolumePosition,
        Vector3 VolumeScale) : ActorParam;

    public record DamageTextParams(Vector3 Position, int Damage, float DespawnTime) : ActorParam;

    public record HealTextParams(Vector3 Position, int Damage, float DespawnTime) : ActorParam;
}