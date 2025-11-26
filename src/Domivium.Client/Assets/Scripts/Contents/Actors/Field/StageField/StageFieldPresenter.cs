using System.Collections.Generic;
using Domivium.Client.Core.Factory;
using UnityEngine;

namespace Domivium.Client.Contents.Actors
{
    public sealed class StageFieldPresenter : FieldPresenter<StageField>
    {
        public IReadOnlyDictionary<ushort, Vector2> StageProp => Actor.StageProps;

        public StageFieldPresenter(StageField actor, ISystemFactory systemFactory) : base(actor, systemFactory) { }
    }
}