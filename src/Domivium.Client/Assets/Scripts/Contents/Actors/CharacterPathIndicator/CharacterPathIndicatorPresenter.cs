using System;
using Domivium.Client.Contents.ReadModels;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Message;
using MessagePipe;
using R3;

namespace Domivium.Client.Contents.Actors
{
    public class CharacterPathIndicatorPresenter : ActorPresenter<CharacterPathIndicator>
    {
        public CharacterPathIndicatorPresenter(
            Guid id,
            CharacterPathIndicator actor,
            IPublisher<ActorTagMessage> tagPublisher,
            IBattleReadModel read)
            : base(id, actor, tagPublisher)
        {
            read.PickedCharacter.Subscribe(actor.SetCharacter).AddTo(ref DisposableBag);
            read.PreviewPosition.Subscribe(actor.SetTargetPosition).AddTo(ref DisposableBag);
        }
    }
}