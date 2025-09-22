using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Core.Audio;
using Domivium.Client.Core.Exceptions;
using Domivium.Client.Core.UI.Contract;
using Domivium.Client.Core.UI.Navigation;
using Domivium.Client.Core.UI.View;
using UnityEngine;

namespace Domivium.Client.Core.UI.Presenter
{
    public class UIPresenter<TView, TMessage> : Disposable, IUIPresenter
        where TView : IUIView<TMessage>
        where TMessage : IUIMessage
    {
        private bool _initialized;

        protected readonly TView View;
        protected readonly IUINavigation Navigation;
        protected readonly IAudioController AudioController;

        public virtual bool DeactivateBehindView { get; }

        protected UIPresenter(TView view, IUINavigation navigation, IAudioController audioController)
        {
            if (this is not TMessage message)
            {
                throw new InvalidOperationException($"returned {GetType().Name}, expected {typeof(TMessage).Name}");
            }

            View = view;
            View.AttachMessage(message);
            Navigation = navigation;
            AudioController = audioController;
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