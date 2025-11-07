using Domivium.Client.Core.Factory;

namespace Domivium.Client.Contents.Actors
{
    public class HealTextPresenter : VFXPresenter<HealText>
    {
        public HealTextPresenter(HealText actor, ISystemFactory systemFactory) : base(actor, systemFactory) { }
    }
}