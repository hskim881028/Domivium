using Domivium.Client.Contents.ReadModels;
using Domivium.Client.Core.Factory;
using R3;

namespace Domivium.Client.Contents.Actors
{
    public class CharacterPathIndicatorPresenter : ActorPresenter<CharacterPathIndicator>
    {
        public CharacterPathIndicatorPresenter(
            CharacterPathIndicator actor,
            ISystemFactory systemFactory,
            IBattleReadModel read)
            : base(actor, systemFactory)
        {
            read.PickedCharacter.Subscribe(actor.SetCharacter).AddTo(ref DisposableBag);
            read.PreviewPosition.Subscribe(actor.SetTargetPosition).AddTo(ref DisposableBag);
        }
    }
}