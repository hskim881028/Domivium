using System;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.State;
using Domivium.Client.Data.Rarity;
using Domivium.Client.Data.Stat;
using R3;
using UnityEngine;

namespace Domivium.Client.Core.Battle
{
    public interface IBattleSystem : ITicker, IDisposable
    {
        public ushort Uid { get; }
        public ActorId ActorId { get; }
        public int Id { get; }
        public RarityType Rarity { get; }
        public StatSet Stat { get; }
        public GaugeSet Gauge { get; }
        public Vector2 PrePosition { get; }
        public Vector2 Position { get; }
        public Vector2 MuzzlePosition { get; }
        public Vector2 ColliderSize { get; }
        public StateTag State { get; }
        public ReactiveCommand<BattleEffectContext> OnAppliedEffect { get; }
        public ReactiveCommand<BattleAbilitySpec> OnActivateAbility { get; }
        public ReadOnlyReactiveProperty<Vector2> Direction { get; }
        public ReadOnlyReactiveProperty<Vector2> LookAt { get; }
        public bool ContainsEffectTag(BattleEffectTag tag);
        public void Initialize(ushort uid, ActorId actorId, int id, RarityType rarity, Vector2 position);
        public void Reset();
        public void SetPosition(Vector2 position);
        public void SetDirection(Vector2 direction);
        public void SetLookAt(Vector2 lookAt);
        public void GrantAbility(BattleAbility ability);
        public bool CanActivateAbility(BattleAbilityId abilityId);
        public bool TryActivateAbility(ref BattleAbilityContext context);
        public void ActivateEffect(BattleEffectSpec spec);
        public void DeactivateEffect(BattleEffectId effectId);
        public bool TryAddHistory(ushort uid);
    }
}