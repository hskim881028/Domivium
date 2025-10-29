using R3;

namespace Domivium.Client.Core.Context
{
    public sealed class StageContext : Disposable
    {
        private readonly ReactiveProperty<StageMode> _mode;

        public ReadOnlyReactiveProperty<StageMode> Mode => _mode;

        public StageContext()
        {
            _mode = new ReactiveProperty<StageMode>(StageMode.Prepare).AddTo(ref DisposableBag);
        }

        internal void SetMode(StageMode id)
        {
            _mode.Value = id;
        }
    }
}