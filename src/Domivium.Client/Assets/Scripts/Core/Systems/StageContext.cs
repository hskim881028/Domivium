using Domivium.Client.Core.Scene;
using R3;

namespace Domivium.Client.Core.Systems
{
    public class AppContext : IAppContext
    {
        private readonly ReactiveProperty<SceneScopeId> _scene = new();
        private readonly ReactiveProperty<StageMode> _mode = new();

        public ReadOnlyReactiveProperty<SceneScopeId> Scene => _scene;
        public ReadOnlyReactiveProperty<StageMode> Mode => _mode;

        public void SetScene(SceneScopeId sceneScopeId)
        {
            _scene.Value = sceneScopeId;
        }

        public void SetMode(StageMode mode)
        {
            _mode.Value = mode;
        }
    }
}