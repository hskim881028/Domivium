using Domivium.Client.Core.Factory;

namespace Domivium.Client.Contents.Actors
{
    public class PropPresenter : ActorPresenter<Prop>
    {
        public PropPresenter(Prop actor, ISystemFactory systemFactory) : base(actor, systemFactory) { }
    }
}