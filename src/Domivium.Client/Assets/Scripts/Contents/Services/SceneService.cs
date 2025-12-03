using Domivium.Client.Contents.DI;
using Domivium.Client.Contents.DI.Scene;
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
            if (sceneScopeId == SceneScopeIds.Login)
            {
                _sceneScopeManager.LoadScope<Login>(sceneScopeId);
            }
            else if (sceneScopeId == SceneScopeIds.Bootstrap)
            {
                _sceneScopeManager.LoadScope<Bootstrap>(sceneScopeId);
            }
            else if (sceneScopeId == SceneScopeIds.Lobby)
            {
                _sceneScopeManager.LoadScope<Lobby>(sceneScopeId);
            }
            else if (sceneScopeId == SceneScopeIds.Stage)
            {
                _sceneScopeManager.LoadScope<Stage>(sceneScopeId);
            }
        }
    }
}