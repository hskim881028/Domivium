using System.Collections.Generic;
using Domivium.Client.Core.Factory;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Domivium.Client.Contents.Actors
{
    public sealed class StageFieldPresenter : ActorPresenter<StageField>
    {
        public Tilemap ColliderGrid => Actor.ColliderGrid;
        public IReadOnlyDictionary<ushort, Vector2> StageProp => Actor.StageProps;

        public StageFieldPresenter(StageField actor, ISystemFactory systemFactory) : base(actor, systemFactory) { }
    }
}