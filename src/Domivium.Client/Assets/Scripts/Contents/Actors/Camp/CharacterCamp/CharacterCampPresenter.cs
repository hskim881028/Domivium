using Domivium.Client.Contents.Actors.Generated;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Factory;

namespace Domivium.Client.Contents.Actors
{
    public class CharacterCampPresenter : CampPresenter<CharacterCamp>
    {
        public override ActorId ActorId => ActorIds.CharacterCamp;
        public CharacterCampPresenter(CharacterCamp actor, ISystemFactory systemFactory) : base(actor, systemFactory) { }
    }
}