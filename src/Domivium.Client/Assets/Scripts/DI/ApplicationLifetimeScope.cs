using Domivium.Client.Contents.Command;
using Domivium.Client.Contents.Services;
using Domivium.Client.Core.Command;
using Domivium.Client.Network;
using Domivium.Client.Network.ClientFilters;
using MagicOnion.Client;
using MessagePipe;
using VContainer;
using VContainer.Unity;

namespace Domivium.Client.DI
{
    public class ApplicationLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            Message(builder);
            Network(builder, Lifetime.Singleton);
            Command(builder, Lifetime.Singleton);

            builder.Register<ApplicationEntry>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
        }

        private static void Message(IContainerBuilder builder)
        {
            var options = builder.RegisterMessagePipe();
        }

        private void Network(IContainerBuilder builder, Lifetime lifetime)
        {
            builder.Register<IClientFilter, LoggingFilter>(lifetime).AsSelf();
            builder.Register<IClientFilter, RetryFilter>(lifetime).AsSelf();
            builder.Register<IResponseHandler, ResponseHandler>(lifetime);
            builder.Register<INetworkConnection, NetworkConnection>(lifetime);
            builder.Register<NetworkService>(lifetime);
        }

        private void Command(IContainerBuilder builder, Lifetime lifetime)
        {
            builder.Register<ICommandExecutor, CommandExecutor>(lifetime);
            builder.Register<LoginCmd>(lifetime);
        }

        private void Components(IContainerBuilder builder, Lifetime lifetime) { }

        private void Repository(IContainerBuilder builder, Lifetime lifetime) { }

        private void Services(IContainerBuilder builder, Lifetime lifetime) { }

        private static void Scene(IContainerBuilder builder, Lifetime lifetime) { }

        private void UI(IContainerBuilder builder, Lifetime lifetime) { }
    }
}