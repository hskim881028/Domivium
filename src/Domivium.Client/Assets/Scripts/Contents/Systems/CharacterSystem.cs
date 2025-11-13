using Domivium.Client.Contents.Battle;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Systems;
using R3;
using UnityEngine;

namespace Domivium.Client.Contents.Systems
{
    public sealed class CharacterSystem : Disposable, ICharacterSystem, ICharacterSystemCommand
    {
        private readonly ReactiveProperty<Vector2> _direction = new();
        private readonly ReactiveProperty<Vector2> _lookAt = new();

        public ReactiveCommand<IBattleSystem> OnInitialize { get; } = new();
        public ReactiveCommand<BattleTag> OnBattleTag { get; } = new();
        public ReadOnlyReactiveProperty<Vector2> OnTurn => _direction;
        public ReadOnlyReactiveProperty<Vector2> OnLookAt => _lookAt;

        public void Initialize(IBattleSystem character)
        {
            OnInitialize.Execute(character);
            _lookAt.Value = Vector2.one * 0.1f;
        }

        public bool SetDirection(Vector2 value)
        {
            if (value.sqrMagnitude > 1f)
            {
                value.Normalize();
            }

            _direction.Value = value;
            return true;
        }

        public bool LookAt(Vector2 value)
        {
            _lookAt.Value = value;
            var tag = _lookAt.Value.sqrMagnitude > Constant.CanAttackRange ? BattleTags.Firing : BattleTags.Aiming;
            OnBattleTag.Execute(tag);
            return true;
        }

        public bool Reload()
        {
            _lookAt.Value = Vector2.zero;
            OnBattleTag.Execute(BattleTags.Idle);
            return true;
        }

        public bool Avoid()
        {
            OnBattleTag.Execute(BattleTags.Avoid);
            return true;
        }

        public bool SelectItem(Vector2 value)
        {
            return true;
        }

        public bool UseItem()
        {
            return true;
        }

        public bool OpenInventory()
        {
            return true;
        }

        public bool Interact()
        {
            return true;
        }
    }
}