using Domivium.Client.Core.Message;
using MessagePipe;
using R3;
using DisposableBag = R3.DisposableBag;

namespace Domivium.Client.Core.Input
{
    public abstract class InputComposition : IInputComposition
    {
        private readonly ISubscriber<InputMessage> _subscriber;
        private DisposableBag _disposable;
        private bool _isDisposed;

        protected readonly InputRouter Router;
        protected abstract void OnDispose();

        protected InputComposition(InputRouter router, ISubscriber<InputMessage> subscriber)
        {
            Router = router;
            _subscriber = subscriber;
        }

        protected void Subscribe()
        {
            _subscriber.Subscribe(Router.OnInput).AddTo(ref _disposable);
        }

        public void Dispose()
        {
            if (_isDisposed) return;

            _isDisposed = true;
            OnDispose();
            _disposable.Dispose();
        }
    }
}