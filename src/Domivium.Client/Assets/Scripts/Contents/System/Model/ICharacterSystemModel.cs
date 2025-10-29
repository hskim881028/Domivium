using R3;
using UnityEngine;

namespace Domivium.Client.Contents.System.Model
{
    public interface ICharacterSystemModel
    {
        public ReadOnlyReactiveProperty<Vector2> OnMove { get; }
        public ReadOnlyReactiveProperty<Vector2> OnLookAt { get; }
        public ReactiveCommand<Vector2> OnFire { get; }
        public ReactiveCommand<float> OnAvoid { get; }
    }
}