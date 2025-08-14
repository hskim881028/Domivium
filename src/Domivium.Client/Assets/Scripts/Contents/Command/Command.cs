using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Services;
using Domivium.Client.Core.Command;
using VContainer;

namespace Domivium.Client.Contents.Command
{
    public abstract class Command : ICommand
    {
        protected NetworkService NetworkService { get; private set; }

        [Inject]
        public void Construct(NetworkService networkService)
        {
            NetworkService = networkService;
        }

        public abstract UniTask<bool> ExecuteAsync();
        public abstract UniTask PostExecuteAsync();
    }
}