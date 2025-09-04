using R3;
using UnityEngine;

namespace Domivium.Client.Contents.ReadModels
{
    public interface IBattleReadModel
    {
        public ReadOnlyReactiveProperty<Transform> PickedCharacter { get; }
        public ReadOnlyReactiveProperty<Vector3> PreviewPosition { get; }
        public ReadOnlyReactiveProperty<Vector3> TargetPosition { get; }
    }
}