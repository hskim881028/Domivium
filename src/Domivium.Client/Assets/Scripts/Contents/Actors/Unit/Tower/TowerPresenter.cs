using System;
using Domivium.Client.Core.Message;
using MessagePipe;

namespace Domivium.Client.Contents.Actors
{
    public class TowerPresenter : UnitPresenter<Tower>
    {
        public TowerPresenter(Guid id, Tower actor, IPublisher<ActorTagMessage> tagPublisher) : base(id, actor, tagPublisher) { }
    }
}