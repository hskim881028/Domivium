using Domivium.Client.Contents.Actors.Generated;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Factory;

namespace Domivium.Client.Contents.Actors
{
    public class ProjectileEffectPresenter : VFXPresenter<ProjectileEffect>
    {
        public override ActorId ActorId => ActorIds.ProjectileEffect;

        public ProjectileEffectPresenter(ProjectileEffect actor, ISystemFactory systemFactory)
            : base(actor, systemFactory) { }
    }
}