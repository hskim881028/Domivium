using Domivium.Client.Contents.ReadModels;
using Domivium.Client.Core.Actors;
using R3;

namespace Domivium.Client.Contents.Actors
{
    public class CharacterPresenter : ActorPresenter<Character>
    {
        public CharacterPresenter(
            Character actor,
            IBattleReadModel read)
            : base(actor)
        {
            read.TargetPosition.Subscribe(actor.SetTargetPosition).AddTo(ref Disposable);
        }

        protected override void OnDispose() { }
    }
}