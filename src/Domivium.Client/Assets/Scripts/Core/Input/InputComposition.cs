using Domivium.Client.Core.Message;
using MessagePipe;
using R3;

namespace Domivium.Client.Core.Input
{
    public abstract class InputComposition : Disposable, IInputComposition
    {
        private readonly ISubscriber<InputMessage> _subscriber;

        protected readonly InputRouter Router;

        protected InputComposition(InputRouter router, ISubscriber<InputMessage> subscriber)
        {
            Router = router;
            _subscriber = subscriber;
        }

        protected void Subscribe()
        {
            _subscriber.Subscribe(Router.OnInput).AddTo(ref DisposableBag);
        }
    }
}