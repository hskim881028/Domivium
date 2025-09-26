using Domivium.Client.Contents.Actors.Generated;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Factory;

namespace Domivium.Client.Contents.Actors
{
    public class MonsterCampPresenter : CampPresenter<MonsterCamp>
    {
        public override ActorId ActorId => ActorIds.MonsterCamp;
        public MonsterCampPresenter(MonsterCamp actor, ISystemFactory systemFactory) : base(actor, systemFactory) { }
    }
}