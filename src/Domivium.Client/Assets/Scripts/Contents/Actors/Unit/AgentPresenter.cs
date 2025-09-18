using System;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Message;
using Domivium.Client.Data.Stat;
using MessagePipe;

namespace Domivium.Client.Contents.Actors
{
    public abstract class AgentPresenter<TAgent> : UnitPresenter<TAgent>, IAgentPresenter where TAgent : Agent
    {
        protected AgentPresenter(Guid id, TAgent actor, IPublisher<ActorTagMessage> tagPublisher)
            : base(id, actor, tagPublisher) { }

        protected override void OnSpeedStatChanged()
        {
            var speed = BattleSystem.Stat.Value(StatId.Speed);
            Actor.SetSpeed(speed);
            base.OnSpeedStatChanged();
        }
    }
}