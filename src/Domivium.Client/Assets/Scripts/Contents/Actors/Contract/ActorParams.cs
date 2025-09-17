using System.Collections.Generic;
using Domivium.Client.Core.Actors.Contract;
using Domivium.Client.Core.Battle;
using Domivium.Client.Data.Row;
using UnityEngine;

namespace Domivium.Client.Contents.Actors.Contract
{
    public record ActorParams(IReadOnlyCollection<Vector3Int> Cells) : ActorParam;

    public record UnitParams(CharacterRow CharacterRow, IReadOnlyList<BattleAbilitySpec> Abilities) : ActorParam;

    public record TowerParams(Vector3Int StartCell) : ActorParam;
    public record DamageTextParams(Vector3 Position, int Damage, float DespawnTime) : ActorParam;
}