using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Factory;

namespace Domivium.Client.Contents.Actors
{
    public abstract class VFXPresenter<TVFX> : ActorPresenter<TVFX>, IVFXPresenter where TVFX : VFX
    {
        protected VFXPresenter(TVFX actor, ISystemFactory systemFactory)
            : base(actor, systemFactory) { }
    }
}