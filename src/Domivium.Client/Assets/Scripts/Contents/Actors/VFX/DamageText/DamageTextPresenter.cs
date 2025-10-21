using Domivium.Client.Contents.Actors.Generated;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Factory;

namespace Domivium.Client.Contents.Actors
{
    public class DamageTextPresenter : VFXPresenter<DamageText>
    {
        public override ActorId ActorId => ActorIds.DamageText;
        public DamageTextPresenter(DamageText actor, ISystemFactory systemFactory) : base(actor, systemFactory) { }
    }
}