using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Core.UI.Contract;
using UnityEngine;

namespace Domivium.Client.Core.UI.View
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UIView<TMessage> : UIViewBase, IUIView<TMessage> where TMessage : IUIMessage
    {
        protected TMessage Message;
        private CanvasGroup _canvasGroup;
        private RectTransform _rectTransform;

        protected RectTransform Self => _rectTransform ?? GetComponent<RectTransform>();

        private void Awake()
        {
            OnAwake();
        }

        protected virtual void OnAwake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.alpha = 0;
        }

        public void AttachMessage(TMessage message)
        {
            Message = message;
        }

        public void SetParent(Transform parent)
        {
            Self.SetParent(parent);
            Self.localPosition = Vector3.zero;
            Self.localScale = Vector3.one;
            Self.offsetMin = Vector2.zero;
            Self.offsetMax = Vector2.zero;
            Self.anchorMin = Vector2.zero;
            Self.anchorMax = Vector2.one;
            Self.pivot = Vector2.one * 0.5f;
        }

        public virtual UniTask InitializeAsync(CancellationToken token)
        {
            // TODO: Need to implement
            return UniTask.CompletedTask;
        }

        public virtual UniTask ShowAsync(CancellationToken token, UIParam param, bool immediately = false)
        {
            // TODO: Need to implement
            // await _content.ShowAsync(token, immediately);
            _canvasGroup.interactable = false;
            _canvasGroup.alpha = 1;
            _canvasGroup.interactable = true;
            return UniTask.CompletedTask;
        }

        public virtual UniTask HideAsync(CancellationToken token, bool immediately = false)
        {
            _canvasGroup.interactable = false;
            // TODO: Need to implement
            // await _content.HideAsync(token, immediately);
            gameObject.SetActive(false);
            return UniTask.CompletedTask;
        }

        public void Activate(float opacity = 1)
        {
            _canvasGroup.alpha = opacity;
            // TODO: Need to implement
            // _content.transform.localScale = Vector3.one;
            gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }
    }
}