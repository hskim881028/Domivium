using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Utility;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

namespace Domivium.Client.Core.Scene
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasScaler))]
    public class UIRootScope : LifetimeScope
    {
        private Canvas _canvas;
        private CanvasScaler _canvasScaler;

        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
            gameObject.layer = Layer.UI;
            transform.SetParent(null);
            SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());

            var cameraSet = Parent.Container.Resolve<CameraRig>();
            _canvas = GetComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceCamera;
            _canvas.worldCamera = cameraSet.UI;
            _canvas.vertexColorAlwaysGammaSpace = true;

            _canvasScaler = GetComponent<CanvasScaler>();
            _canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            _canvasScaler.referenceResolution = new Vector2(1920, 1080);
            _canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            _canvasScaler.matchWidthOrHeight = 0;
            _canvasScaler.referencePixelsPerUnit = 100;
        }
    }
}