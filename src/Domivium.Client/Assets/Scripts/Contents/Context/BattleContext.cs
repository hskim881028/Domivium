using Domivium.Client.Contents.Battle;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Context;
using R3;
using UnityEngine;

namespace Domivium.Client.Contents.Context
{
    public sealed class BattleContext : IBattleContext
    {
        private readonly ReactiveProperty<int> _loadedProjectile = new();
        private readonly ReactiveProperty<int> _remainProjectile = new();
        private readonly ReactiveProperty<Vector2> _direction = new();
        private readonly ReactiveProperty<Vector2> _lookAt = new();
        private readonly ReactiveProperty<BattleTag> _battleTag = new();

        public ReadOnlyReactiveProperty<int> LoadedProjectile => _loadedProjectile;
        public ReadOnlyReactiveProperty<int> RemainProjectile => _remainProjectile;
        public ReadOnlyReactiveProperty<Vector2> OnTurn => _direction;
        public ReadOnlyReactiveProperty<Vector2> OnLookAt => _lookAt;
        public ReadOnlyReactiveProperty<BattleTag> OnBattleTag => _battleTag;

        public void Attack()
        {
            _loadedProjectile.Value -= 1;
        }

        public void Reload(int capacity)
        {
            if (capacity <= 0) return;

            var need = capacity - _loadedProjectile.Value;
            var refill = _remainProjectile.CurrentValue < need
                ? _remainProjectile.CurrentValue
                : need;

            _remainProjectile.Value -= refill;
            _loadedProjectile.Value += refill;
        }

        public void Stop()
        {
            _direction.Value = Vector2.zero;
            _lookAt.Value = Vector2.zero;
            _battleTag.Value = BattleTags.Idle;
            _battleTag.ForceNotify();
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
            switch (_lookAt.Value.sqrMagnitude)
            {
                case > 0 when value.sqrMagnitude <= Constant.CanAttackRange:
                    _battleTag.Value = BattleTags.Aiming;
                    break;
                case > Constant.CanAttackRange:
                    _battleTag.Value = BattleTags.Firing;
                    break;
                default:
                    _battleTag.Value = BattleTags.Idle;
                    break;
            }
            return true;
        }

        public bool Avoid()
        {
            _battleTag.Value = BattleTags.Avoid;
            return true;
        }

        public void RestoreProjectile()
        {
            _remainProjectile.Value += _loadedProjectile.Value;
            _loadedProjectile.Value = 0;
            Stop();
        }

        public void SetProjectile(int remainProjectile)
        {
            _remainProjectile.Value = remainProjectile;
            _loadedProjectile.Value = 0;
            Stop();
        }
    }
}