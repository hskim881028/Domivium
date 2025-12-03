using System;
using System.Collections.Generic;
using System.Linq;
using Domivium.Client.Core.Context;
using Domivium.Client.Core.Message;
using Domivium.Client.Core.Scene;
using Domivium.Client.Core.UI.Presenter;
using Domivium.Client.Core.UI.View;
using MessagePipe;
using R3;
using VContainer;
using VContainer.Unity;

namespace Domivium.Client.Core.UI
{
    public sealed class UIManager : Disposable, IUIManager
    {
        private readonly LifetimeScope _rooLifetimeScope;
        private readonly Dictionary<UIId, (Type presenter, Type view)> _uiContainer;
        private readonly Dictionary<UILayer, HashSet<UIId>> _uisByLayer;
        private readonly List<UIBehaviour> _prefabs;
        private readonly IAppContext _appContext;
        private readonly Dictionary<Type, UICanvasScope> _canvas = new();
        private readonly Dictionary<UIId, UIScope> _ui = new();
        private UIRootScope _uiRoot;

        public UIManager(
            LifetimeScope rooLifetimeScope,
            Dictionary<UIId, (Type presenter, Type view)> uiContainer,
            Dictionary<UILayer, HashSet<UIId>> uisByLayer,
            List<UIBehaviour> prefabs,
            IAppContext appContext,
            ISubscriber<SceneMessage> sceneSubscriber)
        {
            _rooLifetimeScope = rooLifetimeScope;
            _uiContainer = uiContainer;
            _uisByLayer = uisByLayer;
            _prefabs = prefabs;
            _appContext = appContext;
            sceneSubscriber.Subscribe(OnSceneMessage).AddTo(ref DisposableBag);
        }

        public HashSet<UIId> GetStaticUI(UILayer layer) => _uisByLayer.TryGetValue(layer, out var uis) ? uis.ToHashSet() : new HashSet<UIId>();

        public T Get<T>(UIId id) where T : IUIPresenter
        {
            if (_ui.TryGetValue(id, out var ui))
            {
                return (T)ui.Presenter;
            }

            var canvas = GetCanvas<T>();
            var (presenterType, viewType) = _uiContainer[id];
            var child = canvas.CreateChild<UIScope>(builder =>
                {
                    var prefab = _prefabs.FirstOrDefault(p => viewType.IsAssignableFrom(p.GetType()));
                    if (prefab == null)
                    {
                        throw new InvalidOperationException($"UI prefab not found for view type {viewType.FullName}. Make sure it is listed in UIContainer.");
                    }
                    builder.RegisterComponentInNewPrefab(prefab, Lifetime.Singleton).AsSelf();
                    builder.Register(presenterType, Lifetime.Singleton);
                },
                $"{presenterType.Name.AsUI()}(Scope)");

            var presenter = (T)child.Container.Resolve(presenterType);
            var component = child.Initialize(presenter);
            _ui.Add(id, component);
            return (T)component.Presenter;
        }

        public void Remove(UIId id)
        {
            _ui.Remove(id);
        }

        protected override void OnDispose()
        {
            Clear();
            base.OnDispose();
        }

        private void Clear()
        {
            foreach (var uiScope in _ui.Values.Where(uiScope => uiScope != null))
            {
                uiScope.Dispose();
            }
            _ui.Clear();

            foreach (var canvasScope in _canvas.Values.Where(canvasScope => canvasScope != null))
            {
                canvasScope.Dispose();
            }
            _canvas.Clear();
        }

        private void OnSceneMessage(SceneMessage message)
        {
            switch (message.Type)
            {
                case SceneMessageType.Unload:
                    Clear();
                    break;
                case SceneMessageType.Load:
                    _uiRoot = message.SceneScope.UIScope;
                    _appContext.SetMode(SceneMode.UILoaded);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private UICanvasScope GetCanvas<T>() where T : IUIPresenter
        {
            var uiType = typeof(T);
            if (_canvas.TryGetValue(uiType, out var canvas)) return canvas;

            var child = _rooLifetimeScope.CreateChild<UICanvasScope>(childScopeName: uiType.Name.AsCanvas());
            var component = child.Initialize(uiType, _uiRoot.transform);
            _canvas.Add(uiType, component);
            return component;
        }
    }
}