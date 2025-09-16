using R3;

namespace Domivium.Client.Core.Context
{
    public sealed class StageContext : Disposable
    {
        private readonly ReactiveProperty<StageMode> _mode = new(StageMode.Idle);
        private readonly ReactiveProperty<StagePhase> _phase = new(StagePhase.Idle);

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

        protected override void OnDispose()
        {
            _mode?.Dispose();
            _phase?.Dispose();
        }
    }
}