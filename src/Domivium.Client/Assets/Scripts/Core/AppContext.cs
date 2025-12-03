using Domivium.Client.Core.Scene;
using R3;

namespace Domivium.Client.Core
{
    public class AppContext : IAppContext
    {
        private readonly ReactiveProperty<SceneScopeId> _scene = new();
        private readonly ReactiveProperty<SceneMode> _mode = new();

        public ReadOnlyReactiveProperty<SceneScopeId> Scene => _scene;
        public ReadOnlyReactiveProperty<SceneMode> Mode => _mode;

        public void SetScene(SceneScopeId sceneScopeId)
        {
            _scene.Value = sceneScopeId;
        }

        public void SetMode(SceneMode mode)
        {
            _mode.Value = mode;
        }
    }
}