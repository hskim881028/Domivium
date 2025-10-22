using System;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.State;
using Domivium.Client.Data.Stat;
using R3;
using UnityEngine;

namespace Domivium.Client.Core.Battle
{
    public interface IBattleSystem : ITicker, IDisposable
    {
        public ushort Uid { get; }
        public ActorId ActorId { get; }
        public PawnType Type { get; }
        public PawnRarityType Rarity { get; }
        public StatSet Stat { get; }
        public GaugeSet Gauge { get; }
        public Vector3 UnitPosition { get; }
        public StateTag State { get; }
        public ReadOnlyReactiveProperty<BattleEffectContext> AppliedEffect { get; }
        public bool Contains(BattleTag tag);
        public void Initialize(ushort uid, ActorId actorId, PawnType type, PawnRarityType rarity);
        public void Reset();
        public void GrantAbility(BattleAbility ability);
        public bool TryActivateAbility(ref BattleAbilityContext context);
        public void ActivateEffect(BattleEffectSpec spec);
    }
}