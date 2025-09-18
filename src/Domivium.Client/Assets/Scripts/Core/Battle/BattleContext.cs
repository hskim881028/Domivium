using UnityEngine;

namespace Domivium.Client.Core.Battle
{
    public struct BattleContext
    {
        public BattleSystem Source { get; init; }
        public Transform Unit { get; init; }
        public BattleAbility Ability { get; set; }
        public int Damage { get; set; }
    }
}