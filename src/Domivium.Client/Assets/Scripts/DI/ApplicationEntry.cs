using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Command;
using Domivium.Client.Contents.Services;
using Domivium.Client.Core.Command;
using VContainer.Unity;

namespace Domivium.Client.DI
{
    public sealed class ApplicationEntry : IStartable, ITickable
    {
        private readonly ICommandExecutor _commandExecutor;

        public ApplicationEntry(
            NetworkService networkService,
            ICommandExecutor commandExecutor)
        {
            networkService.Connect();
            _commandExecutor = commandExecutor;
        }

        public void Start()
        {
            _commandExecutor.Enqueue<LoginCmd>();
        }

        public void Tick()
        {
            _commandExecutor.TickAsync().Forget();
        }
    }
}