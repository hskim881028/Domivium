using Domivium.Client.Contents.Actors.Generated;
using Domivium.Client.Contents.ReadModels;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Factory;
using R3;

namespace Domivium.Client.Contents.Actors
{
    public class CharacterPathIndicatorPresenter : ActorPresenter<CharacterPathIndicator>
    {
        public override ActorId ActorId => ActorIds.CharacterPathIndicator;

        public CharacterPathIndicatorPresenter(
            CharacterPathIndicator actor,
            ISystemFactory systemFactory,
            IBattleUserReadModel readModel)
            : base(actor, systemFactory)
        {
            readModel.PickedCharacter.Subscribe(actor.SetCharacter).AddTo(ref DisposableBag);
            readModel.PreviewPosition.Subscribe(actor.SetTargetPosition).AddTo(ref DisposableBag);
        }
    }
}