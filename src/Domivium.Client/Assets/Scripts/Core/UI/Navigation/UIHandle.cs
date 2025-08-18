using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Core.UI.Contract;

namespace Domivium.Client.Core.UI.Navigation
{
    public sealed class UIHandle : IUIHandle
    {
        private UniTaskCompletionSource<UIResult> _closeSource = new();
        private CancellationToken _cancellationToken;
        private UIHandleState _state = UIHandleState.Created;
        private bool _isDisposed;

        private UIHandle() { }

        public static IUIHandle Create => new UIHandle();
        public static IUIHandle Error => Create.Closed(UIResult.Failure);

        public bool IsOpened => _state == UIHandleState.Opened;
        public bool IsClosed => _state == UIHandleState.Closed;

        public void Reset(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            _closeSource = new UniTaskCompletionSource<UIResult>();
            _state = UIHandleState.Created;
        }

        public IUIHandle Opened()
        {
            _state = UIHandleState.Opened;
            return this;
        }

        public IUIHandle Closed(UIResult result)
        {
            _state = UIHandleState.Closed;
            _closeSource.TrySetResult(result);
            return this;
        }

        public async UniTask<UIResult> WaitUntilClosedAsync()
        {
            return await _closeSource.Task.AttachExternalCancellation(_cancellationToken);
        }

        public void Dispose()
        {
            if (_isDisposed) return;

            _isDisposed = true;
            _closeSource.TrySetCanceled();
        }
    }
}