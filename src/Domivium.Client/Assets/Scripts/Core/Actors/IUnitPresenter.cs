using Domivium.Client.Core.Battle;

namespace Domivium.Client.Core.Actors
{
    public interface IUnitPresenter : ITicker
    {
        public BattleSystem BattleSystem { get; }
    }
}