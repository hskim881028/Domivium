using Domivium.Client.Contents.ReadModels;
using Domivium.Client.Core.Actors;
using R3;

namespace Domivium.Client.Contents.Actors
{
    public class CharacterPathIndicatorPresenter : ActorPresenter<CharacterPathIndicator>
    {
        public CharacterPathIndicatorPresenter(
            CharacterPathIndicator actor,
            IBattleReadModel read)
            : base(actor)
        {
            read.PickedCharacter.Subscribe(actor.SetCharacter).AddTo(ref Disposable);
            read.PreviewPosition.Subscribe(actor.SetTargetPosition).AddTo(ref Disposable);
        }

        protected override void OnDispose() { }
    }
}