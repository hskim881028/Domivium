using Domivium.Client.Core.Battle;
using R3;
using UnityEngine;

namespace Domivium.Client.Contents.ReadModels
{
    public interface IBattleUserReadModel
    {
        public ReadOnlyReactiveProperty<IBattleSystem> SelectedCharacter { get; }
        public ReadOnlyReactiveProperty<IBattleSystem> PickedCharacter { get; }
        public ReadOnlyReactiveProperty<Vector3> PreviewPosition { get; }
        public ReadOnlyReactiveProperty<Vector3> TargetPosition { get; }
    }
}