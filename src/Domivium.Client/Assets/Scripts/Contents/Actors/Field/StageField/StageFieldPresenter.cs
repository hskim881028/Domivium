using System.Collections.Generic;
using Domivium.Client.Core.Factory;
using Domivium.Client.Data.Loot;
using UnityEngine;

namespace Domivium.Client.Contents.Actors
{
    public sealed class StageFieldPresenter : FieldPresenter<StageField>
    {
        public IReadOnlyDictionary<LootType, IReadOnlyList<(ushort lootId, Vector2 position)>> Loots => Actor.Loots;

        public StageFieldPresenter(StageField actor, ISystemFactory systemFactory) : base(actor, systemFactory) { }
    }
}