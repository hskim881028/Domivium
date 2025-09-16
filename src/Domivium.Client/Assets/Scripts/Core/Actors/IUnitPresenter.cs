using Domivium.Client.Core.Battle;

namespace Domivium.Client.Core.Actors
{
    public interface IUnitPresenter : IUnitUpdater
    {
        public BattleSystem BattleSystem { get; }
    }
}