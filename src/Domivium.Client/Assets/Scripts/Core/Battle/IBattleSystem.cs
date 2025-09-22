using Domivium.Client.Core.Actors.Unit;
using Domivium.Client.Core.State;
using Domivium.Client.Data.Stat;
using UnityEngine;

namespace Domivium.Client.Core.Battle
{
    public interface IBattleSystem : ITicker
    {
        public UnitType Type { get; }
        public StatSet Stat { get; }
        public GaugeSet Gauge { get; }
        public Transform Unit { get; }
        public Vector3 UnitPosition { get; }
        public StateTag State { get; }
        public bool Contains(BattleTag tag);
        public void Reset();
        public void SetType(UnitType type);
        public void GrantAbility(BattleAbilitySpec ability);
        public bool TryActivateAbility(BattleAbilityId id, ref BattleAbilityContext context);
        public void ActivateEffect(BattleEffectSpec spec);
    }
}