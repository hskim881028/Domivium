using Domivium.Client.Contents.Input.Consumer;
using Domivium.Client.Core.Input;
using Domivium.Client.Core.Message;
using MessagePipe;

namespace Domivium.Client.Contents.Input.Composition
{
    public class StageInputComposition : InputComposition
    {
        private readonly TowerPlacementInputConsumer _towerPlacementInputConsumer;
        private readonly BattleInputConsumer _battleInputConsumer;

        public StageInputComposition(
            InputRouter router,
            ISubscriber<InputMessage> subscriber,
            TowerPlacementInputConsumer towerPlacementInputConsumer,
            BattleInputConsumer battleInputConsumer)
            : base(router, subscriber)
        {
            _towerPlacementInputConsumer = towerPlacementInputConsumer;
            _battleInputConsumer = battleInputConsumer;
            Router.Register(_towerPlacementInputConsumer);
            Router.Register(_battleInputConsumer);
        }

        protected override void OnDispose()
        {
            Router.Unregister(_towerPlacementInputConsumer);
            Router.Unregister(_battleInputConsumer);
        }
    }
}