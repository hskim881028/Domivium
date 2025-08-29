using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Controller;
using Domivium.Client.Contents.Input;
using Domivium.Client.Contents.Services;
using Domivium.Client.Contents.UI;
using Domivium.Client.Core;
using Domivium.Client.Core.Input;
using Domivium.Client.Core.Message;
using Domivium.Client.Core.UI.Navigation;
using MessagePipe;
using R3;

namespace Domivium.Client.Contents.DI.Entry
{
    public sealed class ApplicationEntry : Entry
    {
        private readonly SceneService _sceneService;
        private readonly IUINavigation _uiNavigation;

        public ApplicationEntry(
            CameraService cameraService,
            InputPublisher inputPublisher,
            NetworkService networkService,
            SceneService sceneService,
            IUINavigation uiNavigation,
            IPlayerController playerController,
            IInputComposition inputComposition,
            ISubscriber<SceneUIReadyMessage> sceneUIReadySubscriber)
        {
            if (!AppEnv.LocalMode)
            {
                networkService.Connect();
            }

            _sceneService = sceneService;
            _uiNavigation = uiNavigation;
            sceneUIReadySubscriber.Subscribe(OnSceneUIReady).AddTo(ref Disposable);
        }

        protected override void OnStart()
        {
            _sceneService.Load(SceneScopeIds.Title);
        }

        private void OnSceneUIReady(SceneUIReadyMessage message)
        {
            var id = message.SceneScopeId;
            if (id == SceneScopeIds.Title)
            {
                _uiNavigation.ApplyUILayer(UILayers.Title).Forget();
            }
            else if (id == SceneScopeIds.Lobby)
            {
                _uiNavigation.ApplyUILayer(UILayers.Lobby).Forget();
            }
            else if (id == SceneScopeIds.Stage)
            {
                _uiNavigation.ApplyUILayer(UILayers.Stage).Forget();
            }
        }
    }
}