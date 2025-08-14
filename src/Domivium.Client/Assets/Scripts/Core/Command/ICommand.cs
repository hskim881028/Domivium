using Cysharp.Threading.Tasks;

namespace Domivium.Client.Core.Command
{
    public interface ICommand
    {
        public UniTask<bool> ExecuteAsync();
        public UniTask PostExecuteAsync();
    }
}