using Domivium.Client.Contents.State;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Factory;
using Domivium.Client.Core.Message;
using Domivium.Client.Core.State;
using MessagePipe;
using R3;
using UnityEngine;

namespace Domivium.Client.Contents.Factory
{
    public sealed class SystemFactory : ISystemFactory
    {
        private readonly IPublisher<ActorStateMessage> _statePublisher;
        private readonly IPublisher<BattleCueMessage> _cuePublisher;

        public SystemFactory(IPublisher<ActorStateMessage> statePublisher, IPublisher<BattleCueMessage> cuePublisher)
        {
            _statePublisher = statePublisher;
            _cuePublisher = cuePublisher;
        }

        public IStateSystem CreateState(IActorPresenter presenter) => new StateSystem(presenter, _statePublisher);

        public IBattleSystem CreateBattle(Transform unit, ReadOnlyReactiveProperty<StateTag> tag) => new BattleSystem(unit, tag, _cuePublisher);
    }
}