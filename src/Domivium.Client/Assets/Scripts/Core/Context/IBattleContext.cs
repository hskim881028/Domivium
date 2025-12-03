using Domivium.Client.Core.Battle;
using R3;
using UnityEngine;

namespace Domivium.Client.Core.Context
{
    public interface IBattleContext
    {
        public ReadOnlyReactiveProperty<int> LoadedProjectile { get; }
        public ReadOnlyReactiveProperty<int> RemainProjectile { get; }
        public ReadOnlyReactiveProperty<Vector2> OnTurn { get; }
        public ReadOnlyReactiveProperty<Vector2> OnLookAt { get; }
        public ReadOnlyReactiveProperty<BattleTag> OnBattleTag { get; }
        public void Attack();
        public void Reload(int capacity);
        public void Stop();
        public bool SetDirection(Vector2 value);
        public bool LookAt(Vector2 value);
        public bool Avoid();
        public void RestoreProjectile();
        public void SetProjectile(int remainProjectile);
    }
}