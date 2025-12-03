using System;
using Domivium.Client.Core.Context;
using Domivium.Client.Core.Message;
using JetBrains.Annotations;
using MessagePipe;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Domivium.Client.Core.Scene
{
    [UsedImplicitly]
    public sealed class SceneScopeManager : Disposable, ISceneScopeManager
    {
        private readonly LifetimeScope _root;
        private readonly IAppContext _appContext;
        private readonly IPublisher<SceneMessage> _publisher;
        private SceneScope _current;

        public SceneScopeManager(LifetimeScope root, IAppContext appContext, IPublisher<SceneMessage> publisher)
        {
            _root = root;
            _appContext = appContext;
            _publisher = publisher;
        }

        public void LoadScope<T>(SceneScopeId sceneScopeId) where T : SceneScope
        {
            if (_current != null)
            {
                Unload();
            }

            _appContext.SetScene(sceneScopeId);
            _appContext.SetMode(SceneMode.Loading);
            _current = _root.CreateChild<T>(
                builder => builder.RegisterInstance(sceneScopeId).AsSelf(),
                typeof(T).Name);

            _publisher.Publish(SceneMessage.Load(_current));
        }

        protected override void OnDispose()
        {
            Unload();
            base.OnDispose();
        }

        private void Unload()
        {
            _publisher.Publish(SceneMessage.Unload(_current));

            _current?.Dispose();
            _current = null;

            GC.Collect();
            Resources.UnloadUnusedAssets();
        }
    }
}