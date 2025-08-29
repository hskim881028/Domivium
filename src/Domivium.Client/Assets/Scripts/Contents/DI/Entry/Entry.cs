using System;
using VContainer.Unity;
using R3;

namespace Domivium.Client.Contents.DI.Entry
{
    public abstract class Entry : IStartable, IDisposable
    {
        private bool _isDisposed;

        protected DisposableBag Disposable;
        protected abstract void OnStart();
        protected virtual void OnDispose() { }

        public void Start()
        {
            OnStart();
        }

        public void Dispose()
        {
            if (_isDisposed) return;

            _isDisposed = true;
            OnDispose();
            Disposable.Dispose();
        }
    }
}