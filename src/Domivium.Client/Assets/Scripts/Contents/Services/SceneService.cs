using Domivium.Client.Core.Scene;

namespace Domivium.Client.Contents.Services
{
    public sealed class SceneService
    {
        private readonly ISceneScopeManager _sceneScopeManager;

        public SceneService(ISceneScopeManager sceneScopeManager)
        {
            _sceneScopeManager = sceneScopeManager;
        }

        public void LoadScope(SceneScopeId sceneScopeId)
        {
            _sceneScopeManager.LoadScope(sceneScopeId);
        }
    }
}