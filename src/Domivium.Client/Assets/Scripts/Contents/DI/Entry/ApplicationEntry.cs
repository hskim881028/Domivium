using DG.Tweening;
using Domivium.Client.Contents.Services;
using Domivium.Client.Core;
using Domivium.Client.Core.Input;
using Domivium.Client.Core.Systems;
using Domivium.Client.Core.UI.Navigation;
using UnityEngine;

namespace Domivium.Client.Contents.DI.Entry
{
    public sealed class ApplicationEntry : Entry
    {
        private readonly SceneService _sceneService;

        public ApplicationEntry(
            IUINavigation uiNavigation,
            NetworkService networkService,
            SceneService sceneService,
            EnvironmentService environmentService,
            InputDispatcher inputDispatcher,
            ICameraSystem cameraSystem,
            IInputComposition inputComposition) : base(uiNavigation)
        {
            if (!AppEnv.LocalMode)
            {
                networkService.Connect();
            }

            _sceneService = sceneService;
        }

        protected override void OnStart()
        {
            DOTween.SetTweensCapacity(512, 256);
            Application.targetFrameRate = 60;
            _sceneService.Load(SceneScopeIds.Title);
        }
    }
}