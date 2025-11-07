using Domivium.Client.Core.Factory;

namespace Domivium.Client.Contents.Actors
{
    public class DamageTextPresenter : VFXPresenter<DamageText>
    {
        public DamageTextPresenter(DamageText actor, ISystemFactory systemFactory) : base(actor, systemFactory) { }
    }
}