using System;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Scene;
using Domivium.Client.Contents.Services;
using Domivium.Client.Contents.UI;
using Domivium.Client.Core.Message;
using Domivium.Client.Core.Scene;
using Domivium.Client.Core.UI.Navigation;
using Domivium.Client.Core.Utility;
using JetBrains.Annotations;
using MessagePipe;
using R3;
using VContainer.Unity;
using DisposableBag = R3.DisposableBag;

namespace Domivium.Client.DI
{
    [UsedImplicitly]
    public sealed class ApplicationEntry : IStartable, IDisposable
    {
        private readonly SceneService _sceneService;
        private readonly IUINavigation _uiNavigation;
        private DisposableBag _disposable;
        private bool _isDisposed;

        public ApplicationEntry(
            CameraService cameraService,
            InputEventService inputEventService,
            NetworkService networkService,
            SceneService sceneService,
            IUINavigation uiNavigation,
            ISubscriber<SceneUIReadyMessage> sceneUIReadySubscriber)
        {
            this.Log();
            networkService.Connect();
            _sceneService = sceneService;
            _uiNavigation = uiNavigation;
            sceneUIReadySubscriber.Subscribe(OnSceneUIReady).AddTo(ref _disposable);
        }

        public void Start()
        {
            _sceneService.Load(SceneScopeIds.Title);
        }

        public void Dispose()
        {
            if (_isDisposed) return;

            _isDisposed = true;
            _disposable.Dispose();
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