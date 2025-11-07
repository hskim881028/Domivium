using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.State;
using R3;
using UnityEngine;

namespace Domivium.Client.Core.Factory
{
    public interface ISystemFactory
    {
        public IStateSystem CreateState(IActorPresenter presenter);

        public IBattleSystem CreateBattle(
            Transform pawn,
            Transform muzzle,
            Collider2D collider,
            ReadOnlyReactiveProperty<StateTag> tag);
    }
}