using System;
using Domivium.Client.Core.Actors.Unit;
using Domivium.Client.Core.State;
using Domivium.Client.Data.Stat;
using R3;
using UnityEngine;

namespace Domivium.Client.Core.Battle
{
    public interface IBattleSystem : ITicker, IDisposable
    {
        public ushort Id { get; }
        public UnitType Type { get; }
        public StatSet Stat { get; }
        public GaugeSet Gauge { get; }
        public Transform Unit { get; }
        public Vector3 UnitPosition { get; }
        public StateTag State { get; }
        public ReadOnlyReactiveProperty<BattleEffectContext> AppliedEffect { get; }
        public bool Contains(BattleTag tag);
        public void Initialize(ushort id, UnitType type);
        public void Reset();
        public void GrantAbility(BattleAbility ability);
        public bool TryActivateAbility(ref BattleAbilityContext context);
        public void ActivateEffect(BattleEffectSpec spec);
    }
}