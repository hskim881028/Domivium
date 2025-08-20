using Domivium.Client.Core.Scene;
using JetBrains.Annotations;

namespace Domivium.Client.Contents.Services
{
    [UsedImplicitly]
    public sealed class SceneService
    {
        private readonly ISceneScopeManager _sceneScopeManager;

        public SceneService(ISceneScopeManager sceneScopeManager)
        {
            _sceneScopeManager = sceneScopeManager;
        }

        public void Load(SceneScopeId sceneScopeId)
        {
            _sceneScopeManager.LoadScope(sceneScopeId);
        }
    }
}