using Domivium.Client.Contents.Actors.Generated;
using Domivium.Client.Contents.DI.Container;
using Domivium.Client.Contents.DI.Entry;
using Domivium.Client.Contents.Director;
using Domivium.Client.Contents.Input.Composition;
using Domivium.Client.Contents.Input.Consumer;
using Domivium.Client.Contents.Services;
using Domivium.Client.Contents.UI.Generated;
using Domivium.Client.Core;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Context;
using Domivium.Client.Core.Director;
using Domivium.Client.Core.Input;
using Domivium.Client.Core.Message;
using Domivium.Client.Core.Provider;
using Domivium.Client.Core.Scene;
using Domivium.Client.Core.UI;
using Domivium.Client.Core.UI.Navigation;
using Domivium.Client.Data.Cache;
using Domivium.Client.Data.SecureStore;
using Domivium.Client.Data.Store;
using Domivium.Client.Network;
using Domivium.Client.Network.ClientFilters;
using MagicOnion.Client;
using MessagePipe;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Domivium.Client.Contents.DI.Scene
{
    public sealed class Application : LifetimeScope
    {
        [SerializeField] private ConfigContainer _configContainer;
        [SerializeField] private GlobalActorContainer _globalActorContainer;
        [SerializeField] private UIContainer _uiContainer;
        [SerializeField] private StageActorContainer _stageActorContainer;
        [SerializeField] private StageMapContainer _stageMapContainer;

        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            Environment();
            Messages(builder);
            SecureStore(builder, Lifetime.Singleton);
            Cache(builder, Lifetime.Singleton);
            Network(builder, Lifetime.Singleton);
            Services(builder, Lifetime.Singleton);
            Input(builder, Lifetime.Singleton);
            Scene(builder, Lifetime.Singleton);

            Provider(builder, Lifetime.Singleton);
            GlobalActors(builder, Lifetime.Singleton);
            UI(builder, Lifetime.Singleton);
            Actor(builder, Lifetime.Singleton);
            Stage(builder, Lifetime.Singleton);

            builder.Register<ApplicationEntry>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
        }

        private void Environment()
        {
            AppEnv.LocalMode = _configContainer.LocalMode;
            AppEnv.ServerUrl = _configContainer.ServerUrl;
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
            builder.Register<SceneService>(lifetime);
            builder.Register<EnvironmentService>(lifetime);
            builder.Register<CameraService>(lifetime).AsImplementedInterfaces();
            builder.Register<PointerService>(lifetime).AsImplementedInterfaces();
            builder.Register<TowerPlacementService>(lifetime).AsImplementedInterfaces();
            builder.Register<BattleService>(lifetime).AsImplementedInterfaces();
        }

        private static void Input(IContainerBuilder builder, Lifetime lifetime)
        {
            builder.Register<InputDispatcher>(lifetime);
            builder.Register<InputRouter>(lifetime);
            builder.Register<ApplicationInputConsumer>(lifetime);
            builder.Register<SystemUIInputConsumer>(lifetime);
            builder.Register<StaticUIInputConsumer>(lifetime);
            builder.Register<StackUIInputConsumer>(lifetime);
            builder.Register<IInputComposition, ApplicationInputComposition>(Lifetime.Singleton);
        }

        private static void Scene(IContainerBuilder builder, Lifetime lifetime)
        {
            builder.Register<ISceneScopeManager, SceneScopeManager>(lifetime);
        }

        private void Provider(IContainerBuilder builder, Lifetime lifetime)
        {
            builder.Register<StageMapProvider>(lifetime).WithParameter(_stageMapContainer.Tilemaps);
        }

        private void GlobalActors(IContainerBuilder builder, Lifetime lifetime)
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

        private void Actor(IContainerBuilder builder, Lifetime lifetime)
        {
            builder.Register<IActorSpawner, ActorSpawner>(lifetime)
                .WithParameter(ActorMapping.Actor)
                .WithParameter(_stageActorContainer.Actor);
        }

        private void Stage(IContainerBuilder builder, Lifetime lifetime)
        {
            builder.Register<StageContext>(lifetime);
            builder.Register<IStageDirector, StageDirector>(lifetime);
            builder.Register<IStageMapStore, StageMapStore>(lifetime);
        }
    }
}