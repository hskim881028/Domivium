using Domivium.Client.Contents.Actors.Generated;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Factory;

namespace Domivium.Client.Contents.Actors
{
    public class HealTextPresenter : VFXPresenter<HealText>
    {
        public override ActorId ActorId => ActorIds.HealText;
        public HealTextPresenter(HealText actor, ISystemFactory systemFactory) : base(actor, systemFactory) { }
    }
}