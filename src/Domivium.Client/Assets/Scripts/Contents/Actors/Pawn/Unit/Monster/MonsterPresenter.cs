using Domivium.Client.Contents.Actors.Generated;
using Domivium.Client.Contents.System.Model;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Factory;

namespace Domivium.Client.Contents.Actors
{
    public class MonsterPresenter : UnitPresenter<Monster>
    {
        public override ActorId ActorId => ActorIds.Monster;

        public MonsterPresenter(
            Monster actor,
            ISystemFactory systemFactory,
            IStageSystemModel stageSystemModel)
            : base(actor, systemFactory, stageSystemModel) { }
    }
}