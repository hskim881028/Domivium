using Domivium.Client.Contents.Services;
using Domivium.Client.Contents.UI.Generated;
using Domivium.Client.Core.Message;
using Domivium.Client.Core.Scene;
using Domivium.Client.Core.UI;
using Domivium.Client.Core.UI.Navigation;
using Domivium.Client.Data.Cache;
using Domivium.Client.Data.SecureStore;
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
        [SerializeField] private GlobalActorContainer _globalActorContainer;
        [SerializeField] private UIContainer _uiContainer;

        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            Messages(builder);
            SecureStore(builder, Lifetime.Singleton);
            Cache(builder, Lifetime.Singleton);
            Network(builder, Lifetime.Singleton);
            Services(builder, Lifetime.Singleton);
            Scene(builder, Lifetime.Singleton);
            GlocalActors(builder, Lifetime.Singleton);
            UI(builder, Lifetime.Singleton);

            builder.Register<ApplicationEntry>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
        }

        private static void Messages(IContainerBuilder builder)
        {
            var options = builder.RegisterMessagePipe();
            builder.RegisterMessageBroker<SceneMessage>(options);
            builder.RegisterMessageBroker<SceneUIReadyMessage>(options);
        }

        private static void SecureStore(IContainerBuilder builder, Lifetime lifetime)
        {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            builder.Register<ISecureStore, WindowsDpapiSecureStore>(lifetime);
#endif
        }

        private static void Cache(IContainerBuilder builder, Lifetime lifetime)
        {
            builder.Register<AuthenticationTokenCache>(lifetime);
        }

        private static void Network(IContainerBuilder builder, Lifetime lifetime)
        {
            builder.Register<IClientFilter, AuthenticationClientFilter>(lifetime).AsSelf();
            builder.Register<IClientFilter, LoggingClientFilter>(lifetime).AsSelf();

            builder.Register<IResponseHandler, ResponseHandler>(lifetime);
            builder.Register<INetworkConnection, NetworkConnection>(lifetime);
        }

        private static void Services(IContainerBuilder builder, Lifetime lifetime)
        {
            builder.Register<NetworkService>(lifetime);
            builder.Register<CameraService>(lifetime);
            builder.Register<SceneService>(lifetime);
            builder.Register<InputEventService>(lifetime);
        }

        private static void Scene(IContainerBuilder builder, Lifetime lifetime)
        {
            builder.Register<ISceneScopeManager, SceneScopeManager>(lifetime);
        }

        private void GlocalActors(IContainerBuilder builder, Lifetime lifetime)
        {
            builder.RegisterComponentInNewPrefab(_globalActorContainer.InputEventSystem, lifetime).UnderTransform(transform);
            builder.RegisterComponentInNewPrefab(_globalActorContainer.CameraRig, lifetime).UnderTransform(transform);
            builder.RegisterComponentInNewPrefab(_globalActorContainer.EnvironmentRig, lifetime).UnderTransform(transform);
        }

        private void UI(IContainerBuilder builder, Lifetime lifetime)
        {
            builder.Register<IUIManager, UIManager>(lifetime)
                .WithParameter(UIMapping.UI)
                .WithParameter(UIMapping.UIsByLayer)
                .WithParameter(_uiContainer.UI);
            builder.Register<IUINavigationNodePool, UINavigationNodePool>(lifetime);
            builder.Register<IUINavigation, UINavigation>(lifetime);
        }
    }
}