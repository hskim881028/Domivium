using Domivium.Client.Core.Battle;
using R3;
using UnityEngine;

namespace Domivium.Client.Core.Systems
{
    public interface ICharacterSystem
    {
        public IBattleSystem Character { get; }
        public ReactiveCommand<BattleTag> OnBattleTag { get; }
        public ReadOnlyReactiveProperty<Vector2> OnTurn { get; }
        public ReadOnlyReactiveProperty<Vector2> OnLookAt { get; }
    }
}