using VContainer;
using VContainer.Unity;

namespace Domivium.Client.Core.Scene
{
    public class SceneScope : LifetimeScope
    {
        public SceneScopeId Id { get; set; }

        public UIRootScope UIScope { get; set; }

        public ActorScope ActorScope { get; set; }

        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            builder.RegisterBuildCallback(resolver =>
            {
                Id = resolver.Resolve<SceneScopeId>();
                UIScope = CreateChild<UIRootScope>(childScopeName: $"{GetType().Name} UI");
                ActorScope = CreateChild<ActorScope>(childScopeName: $"{GetType().Name} Actor");
            });
        }

        protected override void OnDestroy()
        {
            UIScope?.Dispose();
            UIScope = null;
            ActorScope?.Dispose();
            ActorScope = null;
            base.OnDestroy();
        }
    }
}