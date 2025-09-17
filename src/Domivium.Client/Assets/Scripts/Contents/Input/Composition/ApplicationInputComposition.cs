using Domivium.Client.Contents.Input.Consumer;
using Domivium.Client.Core.Input;
using Domivium.Client.Core.Message;
using MessagePipe;

namespace Domivium.Client.Contents.Input.Composition
{
    public class ApplicationInputComposition : InputComposition
    {
        private readonly ApplicationInputConsumer _applicationInputConsumer;
        private readonly SystemUIInputConsumer _systemUIInputConsumer;
        private readonly StaticUIInputConsumer _staticUIInputConsumer;
        private readonly StackUIInputConsumer _stackUIInputConsumer;

        public ApplicationInputComposition(
            InputRouter router,
            ISubscriber<InputMessage> subscriber,
            ApplicationInputConsumer applicationInputConsumer,
            SystemUIInputConsumer systemUIInputConsumer,
            StaticUIInputConsumer staticUIInputConsumer,
            StackUIInputConsumer stackUIInputConsumer) : base(router, subscriber)
        {
            _applicationInputConsumer = applicationInputConsumer;
            _systemUIInputConsumer = systemUIInputConsumer;
            _staticUIInputConsumer = staticUIInputConsumer;
            _stackUIInputConsumer = stackUIInputConsumer;
            Router.Register(_applicationInputConsumer);
            Router.Register(_systemUIInputConsumer);
            Router.Register(_staticUIInputConsumer);
            Router.Register(_stackUIInputConsumer);
            Subscribe();
        }

        protected override void OnDispose()
        {
            Router.Unregister(_applicationInputConsumer);
            Router.Unregister(_systemUIInputConsumer);
            Router.Unregister(_staticUIInputConsumer);
            Router.Unregister(_stackUIInputConsumer);
            base.OnDispose();
        }
    }
}