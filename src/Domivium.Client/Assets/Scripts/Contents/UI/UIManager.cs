using System;
using System.Collections.Generic;
using System.Linq;
using Domivium.Client.Contents.UI.Generated;
using Domivium.Client.Core.Message;
using Domivium.Client.Core.Scene;
using Domivium.Client.Core.UI;
using Domivium.Client.Core.UI.Presenter;
using Domivium.Client.Core.UI.View;
using MessagePipe;
using R3;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using DisposableBag = R3.DisposableBag;

namespace Domivium.Client.Contents.UI
{
    public sealed class UIManager : IUIManager
    {
        private readonly LifetimeScope _rooLifetimeScope;
        private readonly Dictionary<UIId, (Type type, string prefabPath)> _presenters;
        private readonly Dictionary<Type, UICanvasScope> _canvas = new();
        private readonly Dictionary<UIId, UIScope> _ui = new();
        private DisposableBag _disposable;
        private UIRootScope _uiRoot;
        private bool _isDisposed;

        public UIManager(
            LifetimeScope rooLifetimeScope,
            Dictionary<UIId, (Type type, string prefabPath)> presenters,
            ISubscriber<SceneMessage> subscriber)
        {
            _rooLifetimeScope = rooLifetimeScope;
            _presenters = presenters;
            subscriber.Subscribe(OnSceneMessage).AddTo(ref _disposable);
        }

        public IReadOnlyList<UIId> GetStaticUI()
        {
            var staticUIIds = typeof(StaticUIId)
                .GetFields()
                .Where(f => f.IsStatic && f.FieldType == typeof(UIId))
                .Select(f => (UIId)f.GetValue(null))
                .ToHashSet();

            var ids = _presenters
                .Where(kv => staticUIIds.Contains(kv.Key))
                .Select(kv => kv.Key)
                .ToList();

            return ids;
        }

        public T Get<T>(UIId id) where T : IUIPresenter
        {
            if (_ui.TryGetValue(id, out var ui))
            {
                return (T)ui.Presenter;
            }

            var canvas = GetCanvas<T>();
            var (type, prefabPath) = _presenters[id];
            var child = canvas.CreateChild<UIScope>(builder =>
                {
                    var prefab = Resources.Load<UIViewBase>(prefabPath);
                    builder.RegisterComponentInNewPrefab(prefab, Lifetime.Singleton).AsSelf();
                    builder.Register(type, Lifetime.Singleton);
                },
                $"{type.Name.AsUI()}");

            var presenter = (T)child.Container.Resolve(type);
            var component = child.gameObject.AddComponent<UIScope>().Initialize(presenter);
            _ui.Add(id, component);
            return (T)component.Presenter;
        }

        public void Remove(UIId id)
        {
            _ui.Remove(id);
        }

        public void Dispose()
        {
            if (_isDisposed) return;

            _isDisposed = true;
            Clear();
            _disposable.Dispose();
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
    }
}