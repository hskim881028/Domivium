using Cysharp.Threading.Tasks;

namespace Domivium.Client.Core.Command
{
    public interface ICommandExecutor
    {
        public void Enqueue<T>() where T : ICommand;
        public UniTask TickAsync();
    }
}