using System;
using Domivium.Client.Contents.ReadModels;
using Domivium.Client.Core.Message;
using MessagePipe;
using R3;

namespace Domivium.Client.Contents.Actors
{
    public class CharacterPresenter : AgentPresenter<Character>
    {
        public CharacterPresenter(
            Guid id,
            Character actor,
            IPublisher<ActorTagMessage> tagPublisher,
            IBattleReadModel read)
            : base(id, actor, tagPublisher)
        {
            read.TargetPosition.Subscribe(actor.SetTargetPosition).AddTo(ref DisposableBag);
        }
    }
}