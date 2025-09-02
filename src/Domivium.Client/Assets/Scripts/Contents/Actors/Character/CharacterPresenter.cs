using Domivium.Client.Core.Actors;

namespace Domivium.Client.Contents.Actors.Character
{
    public class CharacterPresenter : ActorPresenter<Character>
    {
        public CharacterPresenter(Character actor) : base(actor) { }

        protected override void OnDispose() { }
    }
}