using System;
using Domivium.Client.Core.Message;
using JetBrains.Annotations;
using MessagePipe;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Domivium.Client.Core.Scene
{
    [UsedImplicitly]
    public sealed class SceneScopeManager : ISceneScopeManager
    {
        private readonly LifetimeScope _root;
        private readonly IPublisher<SceneMessage> _publisher;
        private SceneScope _current;
        private bool _isDisposed;

        public SceneScopeManager(LifetimeScope root, IPublisher<SceneMessage> publisher)
        {
            _root = root;
            _publisher = publisher;
        }

        public void LoadScope<T>(SceneScopeId sceneScopeId) where T : SceneScope
        {
            if (_current != null)
            {
                Unload();
            }

            _current = _root.CreateChild<T>(
                builder => builder.RegisterInstance(sceneScopeId).AsSelf(),
                typeof(T).Name);
            _publisher.Publish(SceneMessage.Load(_current));
        }

        public void Dispose()
        {
            if (_isDisposed) return;

            _isDisposed = true;
            Unload();
        }

        private void Unload()
        {
            _publisher.Publish(SceneMessage.Unload());

            _current?.Dispose();
            _current = null;

            GC.Collect();
            Resources.UnloadUnusedAssets();
        }
    }
}