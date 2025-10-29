using Domivium.Client.Contents.Actors.Generated;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Context;
using Domivium.Client.Core.Factory;
using R3;
using UnityEngine.Tilemaps;

namespace Domivium.Client.Contents.Actors
{
    public sealed class StageFieldPresenter : ActorPresenter<StageField>
    {
        public Tilemap Grid => Actor.ColliderGrid;
        public override ActorId ActorId => ActorIds.StageField;

        public StageFieldPresenter(
            StageField actor,
            ISystemFactory systemFactory,
            StageContext stageContext)
            : base(actor, systemFactory)
        {
            stageContext.Mode.Subscribe(OnChangeMode).AddTo(ref DisposableBag);
        }

        private void OnChangeMode(StageMode mode) { }
    }
}