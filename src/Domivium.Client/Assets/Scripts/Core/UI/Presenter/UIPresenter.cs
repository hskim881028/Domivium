using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Core.Exceptions;
using Domivium.Client.Core.UI.Contract;
using Domivium.Client.Core.UI.Navigation;
using Domivium.Client.Core.UI.View;
using R3;
using UnityEngine;

namespace Domivium.Client.Core.UI.Presenter
{
    public class UIPresenter<TView, TMessage> : IUIPresenter, IDisposable
        where TView : IUIView<TMessage>
        where TMessage : IUIMessage
    {
        private bool _initialized;
        private bool _isDisposed;

        protected readonly TView View;
        protected readonly IUINavigation Navigation;
        protected DisposableBag Disposable;

        public virtual bool DeactivateBehindView { get; }

        protected UIPresenter(TView view, IUINavigation navigation)
        {
            if (this is not TMessage message)
            {
                throw new Exception($"");
            }

            View = view;
            View.AttachMessage(message);
            Navigation = navigation;
        }

        public void Dispose()
        {
            if (_isDisposed) return;

            _isDisposed = true;
            Disposable.Dispose();
        }

        public void SetParent(Transform parent)
        {
            View.SetParent(parent);
        }

        public virtual async UniTask InitializeAsync(CancellationToken token)
        {
            if (_initialized) return;

            try
            {
                await View.InitializeAsync(token);
                _initialized = true;
            }
            catch (Exception e)
            {
                _initialized = false;
                throw new InitializationFailedException("Initialization failed", e);
            }
        }

        public virtual async UniTask ShowAsync(CancellationToken token, UIParam param, bool immediately = false)
        {
            await View.ShowAsync(token, param, immediately);
        }

        public virtual async UniTask HideAsync(CancellationToken token, bool immediately = false)
        {
            await View.HideAsync(token, immediately);
        }

        public void Activate(float opacity = 1)
        {
            View.Activate(opacity);
        }

        public void Deactivate()
        {
            View.Deactivate();
        }

        public virtual void OnShowEnter() { }

        public virtual void OnShowExit() { }

        public virtual void OnHideEnter() { }

        public virtual void OnHideExit() { }

        protected void Close(UIId id, UIResult result, bool immediately = false)
        {
            Navigation.HideSystemUIAsync(id, result, immediately).Forget();
        }
    }
}