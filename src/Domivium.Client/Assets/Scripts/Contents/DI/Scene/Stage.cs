using Domivium.Client.Contents.DI.Entry;
using Domivium.Client.Core.Scene;
using VContainer;

namespace Domivium.Client.Contents.DI.Scene
{
    public class Stage : SceneScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
            builder.Register<StageEntry>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
        }
    }
}