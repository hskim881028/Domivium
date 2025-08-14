using Domivium.Client.Contents.Command;
using Domivium.Client.Contents.Services;
using Domivium.Client.Core.Command;
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

            builder.Register<ApplicationEntry>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
        }

        private static void Messages(IContainerBuilder builder)
        {
            var options = builder.RegisterMessagePipe();
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
        }

        private void Repositories(IContainerBuilder builder, Lifetime lifetime) { }

        private static void Scene(IContainerBuilder builder, Lifetime lifetime) { }

        private void UI(IContainerBuilder builder, Lifetime lifetime) { }
    }
}