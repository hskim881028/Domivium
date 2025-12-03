using Domivium.Client.Core.Scene;
using R3;

namespace Domivium.Client.Core.Context
{
    public interface IAppContext
    {
        public ReadOnlyReactiveProperty<SceneScopeId> Scene { get; }
        public ReadOnlyReactiveProperty<SceneMode> Mode { get; }
        public void SetScene(SceneScopeId sceneScopeId);
        public void SetMode(SceneMode mode);
    }
}