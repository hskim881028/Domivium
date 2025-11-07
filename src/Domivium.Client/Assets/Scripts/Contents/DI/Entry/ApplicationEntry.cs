using Cysharp.Threading.Tasks;
using DG.Tweening;
using Domivium.Client.Contents.Services;
using Domivium.Client.Contents.UI;
using Domivium.Client.Core;
using Domivium.Client.Core.Input;
using Domivium.Client.Core.Message;
using Domivium.Client.Core.Systems;
using Domivium.Client.Core.UI.Navigation;
using MessagePipe;
using R3;
using UnityEngine;

namespace Domivium.Client.Contents.DI.Entry
{
    public sealed class ApplicationEntry : Entry
    {
        private readonly SceneService _sceneService;
        private readonly IUINavigation _uiNavigation;

        public ApplicationEntry(
            NetworkService networkService,
            SceneService sceneService,
            EnvironmentService environmentService,
            InputDispatcher inputDispatcher,
            ICameraSystem cameraSystem,
            IUINavigation uiNavigation,
            IInputComposition inputComposition,
            ISubscriber<SceneUIReadyMessage> sceneUIReadySubscriber)
        {
            if (!AppEnv.LocalMode)
            {
                networkService.Connect();
            }

            _sceneService = sceneService;
            _uiNavigation = uiNavigation;
            sceneUIReadySubscriber.Subscribe(OnSceneUIReady).AddTo(ref DisposableBag);
        }

        protected override void OnStart()
        {
            DOTween.SetTweensCapacity(512, 256);
            Application.targetFrameRate = 60;
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