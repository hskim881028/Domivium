using Domivium.Client.Core.Actors;

namespace Domivium.Client.Contents.Actors
{
    public class TowerPresenter : ActorPresenter<Tower>
    {
        public TowerPresenter(Tower actor) : base(actor) { }

        protected override void OnDispose() { }
    }
}