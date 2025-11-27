using Domivium.Client.Contents.DI;
using Domivium.Client.Core;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Scene;
using R3;

namespace Domivium.Client.Contents.Services
{
    public sealed class EnvironmentService : Disposable
    {
        private readonly EnvironmentRig _environmentRig;

        public EnvironmentService(EnvironmentRig environmentRig, IAppContext appContext)
        {
            _environmentRig = environmentRig;
            appContext.Scene.Subscribe(OnChangeScene).AddTo(ref DisposableBag);
        }

        private void OnChangeScene(SceneScopeId sceneScopeId)
        {
            if (sceneScopeId == SceneScopeIds.Title)
            {
                _environmentRig.gameObject.SetActive(false);
            }
            else if (sceneScopeId == SceneScopeIds.Lobby)
            {
                _environmentRig.gameObject.SetActive(false);
            }
            else if (sceneScopeId == SceneScopeIds.Stage)
            {
                _environmentRig.gameObject.SetActive(true);
            }
        }
    }
}