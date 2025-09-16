using UnityEngine;

namespace Domivium.Client.Core.Battle
{
    public readonly struct BattleContext
    {
        public BattleSystem Source { get; init; }
        public BattleAbility Ability { get; init; }
        public Transform Unit { get; init; }
        public int Damage { get; init; }

        // public Vector3 HitPoint;
        // public Vector3 Direction;
        // public int StackCount;

        public static BattleContext Create(
            BattleSystem source,
            BattleAbility ability,
            Transform unit,
            int damage = 0)
        {
            return new BattleContext
            {
                Source = source,
                Ability = ability,
                Unit = unit,
                Damage = damage
            };
        }
    }
}