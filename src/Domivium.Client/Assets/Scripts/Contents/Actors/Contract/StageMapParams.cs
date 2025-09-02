using System.Collections.Generic;
using Domivium.Client.Core.Actors.Contract;
using UnityEngine;

namespace Domivium.Client.Contents.Actors.Contract
{
    public record StageMapParams(IReadOnlyCollection<Vector3Int> Cells) : ActorParam;
    public record CharacterParams(Vector3Int StartCell) : ActorParam;
}