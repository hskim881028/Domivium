using VContainer;
using VContainer.Unity;

namespace Domivium.Client.Core.Scene
{
    public class SceneScope : LifetimeScope
    {
        public SceneScopeId Id { get; set; }

        public UIRootScope UIScope { get; set; }

        public ActorRootScope ActorRootScope { get; set; }

        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            builder.RegisterBuildCallback(resolver =>
            {
                Id = resolver.Resolve<SceneScopeId>();
                UIScope = CreateChild<UIRootScope>(childScopeName: $"{GetType().Name} UI");
                ActorRootScope = CreateChild<ActorRootScope>(childScopeName: $"{GetType().Name} Actor");
            });
        }

        protected override void OnDestroy()
        {
            UIScope?.Dispose();
            UIScope = null;
            ActorRootScope?.Dispose();
            ActorRootScope = null;
            base.OnDestroy();
        }
    }
}