using System.Collections.Generic;
using Domivium.Client.Core.Actors.Contract;
using Domivium.Client.Core.Battle;
using UnityEngine;

namespace Domivium.Client.Contents.Actors.Contract
{
    public record ActorParams(IReadOnlyCollection<Vector3Int> Cells) : ActorParam;

    public record UnitParams(UnitContext UnitContext, IReadOnlyList<BattleAbilitySpec> Abilities) : ActorParam;

    public record TowerParams(Vector3Int StartCell, UnitContext UnitContext, IReadOnlyList<BattleAbilitySpec> Abilities) : UnitParams(UnitContext, Abilities);

    public record DamageTextParams(Vector3 Position, int Damage, float DespawnTime) : ActorParam;
}