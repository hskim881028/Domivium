using Domivium.Client.Contents.Input.Consumer;
using Domivium.Client.Core.Input;
using Domivium.Client.Core.Message;
using MessagePipe;

namespace Domivium.Client.Contents.Input.Composition
{
    public class StageInputComposition : InputComposition
    {
        private readonly TowerPlacementInputConsumer _towerPlacementInputConsumer;

        public StageInputComposition(
            InputRouter router,
            ISubscriber<InputMessage> subscriber,
            TowerPlacementInputConsumer towerPlacementInputConsumer) : base(router, subscriber)
        {
            _towerPlacementInputConsumer = towerPlacementInputConsumer;
            Router.Register(_towerPlacementInputConsumer);
        }

        protected override void OnDispose()
        {
            Router.Unregister(_towerPlacementInputConsumer);
        }
    }
}