using Cysharp.Threading.Tasks;

namespace Domivium.Client.Core.Systems
{
    public interface ILobbySystemCommand : ITicker
    {
        public UniTaskVoid RunAsync();
    }
}