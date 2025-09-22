using R3;

namespace Domivium.Client.Core.Context
{
    public sealed class StageContext : Disposable
    {
        private readonly ReactiveProperty<StageMode> _mode;
        private readonly ReactiveProperty<StagePhase> _phase;

        public ReadOnlyReactiveProperty<StageMode> Mode => _mode;
        public ReadOnlyReactiveProperty<StagePhase> Phase => _phase;

        public StageContext()
        {
            _mode = new ReactiveProperty<StageMode>(StageMode.Idle).AddTo(ref DisposableBag);
            _phase = new ReactiveProperty<StagePhase>(StagePhase.Idle).AddTo(ref DisposableBag);
        }

        internal void SetMode(StageMode id)
        {
            _mode.Value = id;
        }

        internal void SetPhase(StagePhase phase)
        {
            _phase.Value = phase;
        }
    }
}