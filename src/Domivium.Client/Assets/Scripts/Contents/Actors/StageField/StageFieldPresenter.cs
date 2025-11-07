using Domivium.Client.Core.Factory;
using UnityEngine.Tilemaps;

namespace Domivium.Client.Contents.Actors
{
    public sealed class StageFieldPresenter : ActorPresenter<StageField>
    {
        public Tilemap Grid => Actor.ColliderGrid;

        public StageFieldPresenter(StageField actor, ISystemFactory systemFactory) : base(actor, systemFactory) { }
    }
}