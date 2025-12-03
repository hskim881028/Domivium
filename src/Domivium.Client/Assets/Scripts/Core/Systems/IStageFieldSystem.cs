using System.Collections.Generic;
using Domivium.Client.Data.Loot;
using UnityEngine;

namespace Domivium.Client.Core.Systems
{
    public interface IStageFieldSystem
    {
        public IReadOnlyDictionary<LootType, IReadOnlyList<(ushort lootId, Vector2 position)>> Loots { get; }
        public Vector3 GetNextPosition(Vector3 position, Vector3 delta, Vector2 collider);
    }
}