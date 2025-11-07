using Cysharp.Threading.Tasks;

namespace Domivium.Client.Core.Systems
{
    public interface IStageSystemCommand : ITicker
    {
        public UniTaskVoid RunAsync(int stageId);
    }
}