using System;
using Cysharp.Threading.Tasks;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Message;
using Domivium.Client.Core.State;
using MessagePipe;
using R3;
using UnityEngine;

namespace Domivium.Client.Contents.State
{
    public class StateSystem : IStateSystem
    {
        private readonly IActorPresenter _presenter;
        private readonly IPublisher<ActorStateMessage> _publisher;
        private readonly ReactiveProperty<StateTag> _tag = new();
        private bool _isDisposed;

        public ReadOnlyReactiveProperty<StateTag> Tag => _tag;
        private readonly Func<StateTag, StateTag, bool> _canTransitTest;

        public StateSystem(IActorPresenter presenter, IPublisher<ActorStateMessage> publisher)
        {
            _presenter = presenter;
            _canTransitTest = _presenter.CanTransitState;
            _publisher = publisher;
        }

        public void TryTransit(StateTag next)
        {
            var current = _tag.CurrentValue;
            if (IsTerminated()) return;

            if (!_canTransitTest(current, next)) return;

            _tag.Value = next;
        }

        public async UniTaskVoid DespawnAsync(float despawnSeconds = 0)
        {
            if (despawnSeconds > 0)
            {
                _tag.Value = StateTags.Die;
                _publisher.Publish(ActorStateMessage.Create(StateTags.Die, _presenter.Id));
                await Awaitable.WaitForSecondsAsync(despawnSeconds);
            }

            _tag.Value = StateTags.Despawn;
            _publisher.Publish(ActorStateMessage.Create(StateTags.Despawn, _presenter.Id));
        }

        public void Terminate()
        {
            if (IsDead()) return;

            _tag.Value = StateTags.Terminated;
            _publisher.Publish(ActorStateMessage.Create(StateTags.Terminated, _presenter.Id));
        }

        private bool IsDead() => _tag.CurrentValue == StateTags.Die || _tag.CurrentValue == StateTags.Despawn;

        private bool IsTerminated() => IsDead() || _tag.CurrentValue == StateTags.Terminated;

        public void Dispose()
        {
            if (_isDisposed) return;

            _isDisposed = true;
            _tag?.Dispose();
        }
    }
}