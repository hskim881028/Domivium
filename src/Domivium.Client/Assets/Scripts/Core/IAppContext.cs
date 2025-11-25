using Domivium.Client.Core.Scene;
using Domivium.Client.Core.Systems;
using R3;

namespace Domivium.Client.Core
{
    public interface IAppContext
    {
        public ReadOnlyReactiveProperty<SceneScopeId> Scene { get; }
        public ReadOnlyReactiveProperty<StageMode> Mode { get; }
        public void SetScene(SceneScopeId sceneScopeId);
        public void SetMode(StageMode mode);
    }
}