using Domivium.Client.Contents.Input.Consumer;
using Domivium.Client.Core.Input;
using Domivium.Client.Core.Message;
using Domivium.Client.Core.Utility;
using MessagePipe;

namespace Domivium.Client.Contents.Input.Composition
{
    public class StageInputComposition : InputComposition
    {
        private readonly BuildPlacementInputConsumer _buildPlacementInputConsumer;

        public StageInputComposition(
            InputRouter router,
            ISubscriber<InputMessage> subscriber,
            BuildPlacementInputConsumer buildPlacementInputConsumer) : base(router, subscriber)
        {
            _buildPlacementInputConsumer = buildPlacementInputConsumer;
            Router.Register(_buildPlacementInputConsumer);
        }

        protected override void OnDispose()
        {
            this.Log();
            Router.Unregister(_buildPlacementInputConsumer);
        }
    }
}