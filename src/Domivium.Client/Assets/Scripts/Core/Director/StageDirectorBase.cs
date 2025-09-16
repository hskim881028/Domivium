using Domivium.Client.Core.Context;

namespace Domivium.Client.Core.Director
{
    public abstract class StageDirectorBase : Disposable, IStageDirector
    {
        private readonly StageContext _stageContext;

        protected StageMode Mode => _stageContext.Mode.CurrentValue;

        protected StagePhase Phase => _stageContext.Phase.CurrentValue;

        protected void WriteMode(StageMode mode) => _stageContext.SetMode(mode);

        protected void WritePhase(StagePhase phase) => _stageContext.SetPhase(phase);

        protected StageDirectorBase(StageContext stageContext)
        {
            _stageContext = stageContext;
        }

        public abstract bool TrySetMode(StageMode mode);

        public abstract bool TrySetPhase(StagePhase phase);
    }
}