using Domivium.Client.Core.Context;

namespace Domivium.Client.Core.Director
{
    public interface IStageDirector
    {
        public bool TrySetMode(StageMode mode);
    }
}