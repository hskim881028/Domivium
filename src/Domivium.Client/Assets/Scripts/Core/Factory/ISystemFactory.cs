using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.State;
using Domivium.Client.Core.Systems;
using R3;
using UnityEngine;

namespace Domivium.Client.Core.Factory
{
    public interface ISystemFactory
    {
        public ISpriteSystem SpriteSystem { get; }

        public IStateSystem CreateState(IActorPresenter presenter);

        public IBattleSystem CreateBattle(
            Transform pawn,
            Transform muzzle,
            Collider2D collider,
            ReadOnlyReactiveProperty<StateTag> tag);
    }
}