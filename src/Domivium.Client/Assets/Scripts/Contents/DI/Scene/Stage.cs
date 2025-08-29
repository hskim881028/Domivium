using Domivium.Client.Contents.DI.Entry;
using Domivium.Client.Contents.Input.Composition;
using Domivium.Client.Contents.Input.Consumer;
using Domivium.Client.Contents.Services;
using Domivium.Client.Core.Input;
using Domivium.Client.Core.Scene;
using Domivium.Client.Data.Store;
using VContainer;

namespace Domivium.Client.Contents.DI.Scene
{
    public class Stage : SceneScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            Store(builder, Lifetime.Singleton);
            Service(builder, Lifetime.Singleton);
            Input(builder, Lifetime.Singleton);

            builder.Register<StageEntry>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
        }

        private static void Store(IContainerBuilder builder, Lifetime lifetime)
        {
            builder.Register<IStageMapStore, StageMapStore>(Lifetime.Singleton);
        }

        private static void Service(IContainerBuilder builder, Lifetime lifetime)
        {
            // builder.Register<ActorSpawnService>(lifetime);
        }

        private static void Input(IContainerBuilder builder, Lifetime lifetime)
        {
            builder.Register<BuildPlacementInputConsumer>(lifetime);
            builder.Register<IInputComposition, StageInputComposition>(lifetime);
        }
    }
}