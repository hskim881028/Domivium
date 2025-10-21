using Domivium.Client.Contents.Actors.Generated;
using Domivium.Client.Contents.ReadModels;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Factory;
using R3;

namespace Domivium.Client.Contents.Actors
{
    public class CharacterSelectIndicatorPresenter : ActorPresenter<CharacterSelectIndicator>
    {
        public override ActorId ActorId => ActorIds.CharacterSelectIndicator;

        public CharacterSelectIndicatorPresenter(
            CharacterSelectIndicator actor,
            ISystemFactory systemFactory,
            IBattleUserReadModel readModel)
            : base(actor, systemFactory)
        {
            readModel.SelectedCharacter.Subscribe(actor.SetCharacter).AddTo(ref DisposableBag);
        }
    }
}