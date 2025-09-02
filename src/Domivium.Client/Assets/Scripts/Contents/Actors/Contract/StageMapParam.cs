using System.Collections.Generic;
using Domivium.Client.Core.Actors.Contract;
using UnityEngine;

namespace Domivium.Client.Contents.Actors.Contract
{
    public record StageMapParam(IReadOnlyCollection<Vector3Int> Cells) : ActorParam;
}