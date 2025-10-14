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
        private readonly BattleCameraInputConsumer _battleCameraInputConsumer;

        public StageInputComposition(
            InputRouter router,
            ISubscriber<InputMessage> subscriber,
            TowerPlacementInputConsumer towerPlacementInputConsumer,
            BattleInputConsumer battleInputConsumer,
            BattleCameraInputConsumer  battleCameraInputConsumer)
            : base(router, subscriber)
        {
            _towerPlacementInputConsumer = towerPlacementInputConsumer;
            _battleInputConsumer = battleInputConsumer;
            _battleCameraInputConsumer = battleCameraInputConsumer;
            Router.Register(_towerPlacementInputConsumer);
            Router.Register(_battleInputConsumer);
            Router.Register(_battleCameraInputConsumer);
        }

        protected override void OnDispose()
        {
            Router.Unregister(_towerPlacementInputConsumer);
            Router.Unregister(_battleInputConsumer);
            Router.Unregister(_battleCameraInputConsumer);
            base.OnDispose();
        }
    }
}