using Domivium.Client.Core.Battle;

namespace Domivium.Client.Core.Actors
{
    public interface IUnitPresenter : IActorPresenter
    {
        public IBattleSystem BattleSystem { get; }
    }
}