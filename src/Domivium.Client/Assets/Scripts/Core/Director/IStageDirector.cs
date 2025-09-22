using Domivium.Client.Core.Context;

namespace Domivium.Client.Core.Director
{
    public interface IStageDirector : ITicker
    {
        public bool TrySetMode(StageMode mode);
        public bool TrySetPhase(StagePhase phase);
    }
}