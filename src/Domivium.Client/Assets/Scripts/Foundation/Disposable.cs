// ReSharper disable CheckNamespace

using System;
using R3;

public class Disposable : IDisposable
{
    private bool _isDisposed;

    protected DisposableBag DisposableBag;

    protected virtual void OnDispose() { }

    public void Dispose()
    {
        if (_isDisposed) return;

        this.Log();
        _isDisposed = true;
        OnDispose();
        DisposableBag.Dispose();
    }
}