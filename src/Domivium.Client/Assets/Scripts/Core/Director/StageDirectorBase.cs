using System;
using Domivium.Client.Core.Context;
using R3;

namespace Domivium.Client.Core.Director
{
    public abstract class StageDirectorBase : IStageDirector, IDisposable
    {
        private readonly StageContext _stageContext;
        private bool _isDisposed;

        protected DisposableBag Disposable;

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

        public void Dispose()
        {
            if (_isDisposed) return;

            _isDisposed = true;
            Disposable.Dispose();
        }
    }
}