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
        private const float Width = 1920f;
        private const float Height = 1080f;
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
            _canvasScaler.referenceResolution = new Vector2(Width, Height);
            _canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            _canvasScaler.referencePixelsPerUnit = 100;
            _canvasScaler.matchWidthOrHeight = 1;

            const float referenceAspect = Width / Height;
            var currentAspect = (float)Screen.width / Screen.height;
            _canvasScaler.matchWidthOrHeight = currentAspect switch
            {
                > referenceAspect => 1f,
                < referenceAspect => 0f,
                _ => 0.5f
            };
        }
    }
}