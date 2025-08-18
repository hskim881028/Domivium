using Domivium.Client.Contents.Command;
using Domivium.Client.Contents.Services;
using Domivium.Client.Contents.UI;
using Domivium.Client.Contents.UI.Generated;
using Domivium.Client.Core.Command;
using Domivium.Client.Core.Message;
using Domivium.Client.Core.Scene;
using Domivium.Client.Core.UI;
using Domivium.Client.Core.UI.Navigation;
using Domivium.Client.Network;
using Domivium.Client.Network.ClientFilters;
using MagicOnion.Client;
using MessagePipe;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Domivium.Client.DI
{
    public sealed class ApplicationLifetimeScope : LifetimeScope
    {
        [SerializeField] private GlobalActors _globalActors;

        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            Messages(builder);
            Network(builder, Lifetime.Singleton);
            Command(builder, Lifetime.Singleton);
            GlocalActors(builder, Lifetime.Singleton);
            Services(builder, Lifetime.Singleton);
            Scene(builder, Lifetime.Singleton);
            UI(builder, Lifetime.Singleton);

            builder.Register<ApplicationEntry>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
        }

        private static void Messages(IContainerBuilder builder)
        {
            var options = builder.RegisterMessagePipe();
            builder.RegisterMessageBroker<SceneMessage>(options);
        }

        private void Network(IContainerBuilder builder, Lifetime lifetime)
        {
            builder.Register<IClientFilter, LoggingFilter>(lifetime).AsSelf();
            builder.Register<IClientFilter, RetryFilter>(lifetime).AsSelf();
            builder.Register<IResponseHandler, ResponseHandler>(lifetime);
            builder.Register<INetworkConnection, NetworkConnection>(lifetime);
        }

        private void Command(IContainerBuilder builder, Lifetime lifetime)
        {
            builder.Register<ICommandExecutor, CommandExecutor>(lifetime);
            builder.Register<LoginCmd>(lifetime);
            builder.Register<EnterStageCmd>(lifetime);
        }

        private void GlocalActors(IContainerBuilder builder, Lifetime lifetime)
        {
            builder.RegisterComponentInNewPrefab(_globalActors.InputEventSystem, lifetime).UnderTransform(transform);
            builder.RegisterComponentInNewPrefab(_globalActors.CameraRig, lifetime).UnderTransform(transform);
            builder.RegisterComponentInNewPrefab(_globalActors.EnvironmentRig, lifetime).UnderTransform(transform);
        }

        private void Services(IContainerBuilder builder, Lifetime lifetime)
        {
            builder.Register<NetworkService>(lifetime);
            builder.Register<CameraService>(lifetime);
            builder.Register<SceneService>(lifetime);
        }

        private static void Scene(IContainerBuilder builder, Lifetime lifetime)
        {
            builder.Register<ISceneScopeManager, SceneScopeManager>(lifetime);
        }

        private void UI(IContainerBuilder builder, Lifetime lifetime)
        {
            builder.Register<IUIManager, UIManager>(lifetime).WithParameter(UIMapping.UI);
            builder.Register<IUINavigationNodePool, UINavigationNodePool>(lifetime);
            builder.Register<IUINavigation, UINavigation>(lifetime);
        }

        private void Repositories(IContainerBuilder builder, Lifetime lifetime) { }
    }
}