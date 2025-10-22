using Domivium.Client.Contents.Actors.Generated;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Factory;

namespace Domivium.Client.Contents.Actors
{
    public class SoulEffectPresenter : VFXPresenter<SoulEffect>
    {
        public override ActorId ActorId => ActorIds.SoulEffect;
        public SoulEffectPresenter(SoulEffect actor, ISystemFactory systemFactory) : base(actor, systemFactory) { }
    }
}