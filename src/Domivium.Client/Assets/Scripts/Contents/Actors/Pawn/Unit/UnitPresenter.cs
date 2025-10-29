using Domivium.Client.Contents.System.Model;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Factory;

namespace Domivium.Client.Contents.Actors
{
    public abstract class UnitPresenter<TUnit> : PawnPresenter<TUnit>, IUnitPresenter where TUnit : Unit
    {
        protected UnitPresenter(TUnit actor, ISystemFactory systemFactory, IStageSystemModel stageSystemModel)
            : base(actor, systemFactory, stageSystemModel) { }
    }
}