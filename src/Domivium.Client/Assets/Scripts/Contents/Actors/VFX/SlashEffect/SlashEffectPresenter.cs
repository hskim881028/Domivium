using Domivium.Client.Contents.Actors.Generated;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Factory;

namespace Domivium.Client.Contents.Actors
{
    public class SlashEffectPresenter : VFXPresenter<SlashEffect>
    {
        public override ActorId ActorId => ActorIds.SlashEffect;
        public SlashEffectPresenter(SlashEffect actor, ISystemFactory systemFactory) : base(actor, systemFactory) { }
    }
}