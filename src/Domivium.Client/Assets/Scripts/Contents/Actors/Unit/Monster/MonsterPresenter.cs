using System;
using Domivium.Client.Core.Message;
using MessagePipe;

namespace Domivium.Client.Contents.Actors
{
    public class MonsterPresenter : AgentPresenter<Monster>
    {
        public MonsterPresenter(
            Guid id,
            Monster actor,
            IPublisher<ActorTagMessage> tagPublisher)
            : base(id, actor, tagPublisher) { }
    }
}