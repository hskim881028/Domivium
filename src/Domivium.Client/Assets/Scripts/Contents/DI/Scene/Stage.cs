using Domivium.Client.Contents.DI.Entry;
using Domivium.Client.Contents.Input.Composition;
using Domivium.Client.Contents.Input.Consumer;
using Domivium.Client.Core.Input;
using Domivium.Client.Core.Scene;
using VContainer;

namespace Domivium.Client.Contents.DI.Scene
{
    public class Stage : SceneScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
            Input(builder, Lifetime.Singleton);

            builder.Register<StageEntry>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
        }

        private static void Input(IContainerBuilder builder, Lifetime lifetime)
        {
            builder.Register<TowerPlacementInputConsumer>(lifetime);
            builder.Register<BattleInputConsumer>(lifetime);
            builder.Register<BattleCameraInputConsumer>(lifetime);
            builder.Register<IInputComposition, StageInputComposition>(lifetime);
        }
    }
}