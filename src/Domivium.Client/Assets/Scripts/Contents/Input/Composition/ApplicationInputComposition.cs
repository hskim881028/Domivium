using Domivium.Client.Contents.Input.Consumer;
using Domivium.Client.Core.Input;
using Domivium.Client.Core.Message;
using Domivium.Client.Core.Utility;
using MessagePipe;

namespace Domivium.Client.Contents.Input.Composition
{
    public class ApplicationInputComposition : InputComposition
    {
        private readonly SystemUIInputConsumer _systemUIInputConsumer;
        private readonly StaticUIInputConsumer _staticUIInputConsumer;
        private readonly StackUIInputConsumer _stackUIInputConsumer;

        public ApplicationInputComposition(
            InputRouter router,
            ISubscriber<InputMessage> subscriber,
            SystemUIInputConsumer systemUIInputConsumer,
            StaticUIInputConsumer staticUIInputConsumer,
            StackUIInputConsumer stackUIInputConsumer) : base(router, subscriber)
        {
            _systemUIInputConsumer = systemUIInputConsumer;
            _staticUIInputConsumer = staticUIInputConsumer;
            _stackUIInputConsumer = stackUIInputConsumer;
            Router.Register(_systemUIInputConsumer);
            Router.Register(_staticUIInputConsumer);
            Router.Register(_stackUIInputConsumer);
            Subscribe();
        }

        protected override void OnDispose()
        {
            Router.Unregister(_systemUIInputConsumer);
            Router.Unregister(_staticUIInputConsumer);
            Router.Unregister(_stackUIInputConsumer);
            this.Log();
        }
    }
}