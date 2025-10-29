using Domivium.Client.Contents.Battle;
using Domivium.Client.Contents.System.Command;
using Domivium.Client.Contents.System.Model;
using Domivium.Client.Core.Battle;
using R3;
using UnityEngine;

namespace Domivium.Client.Contents.System
{
    public sealed class CharacterSystem : Disposable, ICharacterSystemModel, ICharacterSystemCommand
    {
        private readonly ReactiveProperty<Vector2> _direction = new();
        private readonly ReactiveProperty<Vector2> _lookAt = new();

        private IBattleSystem _character;

        public ReadOnlyReactiveProperty<Vector2> OnMove => _direction;
        public ReadOnlyReactiveProperty<Vector2> OnLookAt => _lookAt;
        public ReactiveCommand<Vector2> OnFire { get; } = new();
        public ReactiveCommand<float> OnAvoid { get; } = new();

        public void Initialize(IBattleSystem character)
        {
            _character = character;
            Avoid(true);
        }

        public bool SetDirection(Vector2 value)
        {
            _direction.Value = value;
            return true;
        }

        public bool LookAt(Vector2 value)
        {
            _lookAt.Value = value;
            return true;
        }

        public bool Firing()
        {
            if (_lookAt.Value.sqrMagnitude > Constant.CanAttackRange)
            {
                OnFire.Execute(_lookAt.Value);
            }

            _lookAt.Value = Vector2.zero;
            return true;
        }

        public bool Avoid(bool force = false)
        {
            if (_character.CanActivateAbility(BattleAbilityIds.Avoid, out var cooldown))
            {
                OnAvoid.Execute(cooldown);
                return true;
            }

            if (!force) return false;

            OnAvoid.Execute(cooldown);
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