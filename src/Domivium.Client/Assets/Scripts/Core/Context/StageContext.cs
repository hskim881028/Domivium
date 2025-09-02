using System;
using R3;

namespace Domivium.Client.Core.Context
{
    public sealed class StageContext : IDisposable
    {
        private readonly ReactiveProperty<StageMode> _mode = new(StageMode.Idle);
        private readonly ReactiveProperty<StagePhase> _phase = new(StagePhase.Idle);
        private bool _isDisposed;

        public ReadOnlyReactiveProperty<StageMode> Mode => _mode;
        public ReadOnlyReactiveProperty<StagePhase> Phase => _phase;

        internal void SetMode(StageMode id)
        {
            _mode.Value = id;
        }

        internal void SetPhase(StagePhase phase)
        {
            _phase.Value = phase;
        }

        public void Dispose()
        {
            if (_isDisposed) return;

            _isDisposed = true;
            _mode?.Dispose();
            _phase?.Dispose();
        }
    }
}