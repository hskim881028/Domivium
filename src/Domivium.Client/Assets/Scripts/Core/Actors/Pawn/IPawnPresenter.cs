using Domivium.Client.Core.Battle;

namespace Domivium.Client.Core.Actors
{
    public interface IPawnPresenter : IActorPresenter
    {
        public IBattleSystem BattleSystem { get; }
    }
}