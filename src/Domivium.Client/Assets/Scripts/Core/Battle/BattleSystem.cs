using System.Collections.Generic;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Message;
using Domivium.Client.Core.State;
using Domivium.Client.Data.Stat;
using MessagePipe;
using R3;
using UnityEngine;

namespace Domivium.Client.Core.Battle
{
    public sealed class BattleSystem : IBattleSystem
    {
        private readonly IPublisher<BattleCueMessage> _cuePublisher;
        private readonly Dictionary<BattleAbilityId, BattleAbilitySpec> _abilitySpecs = new();
        private readonly List<BattleEffectSpec> _effectSpecs = new();
        private readonly Queue<BattleEffectSpec> _deactivateEffectSpecs = new();
        private readonly Queue<BattleStatModifier> _statModifiers = new();
        private readonly Queue<BattleGaugeModifier> _gaugeModifiers = new();
        private readonly ReadOnlyReactiveProperty<StateTag> _state;
        private readonly HashSet<BattleEffectTag> _effectTags = new();
        private readonly ReactiveProperty<BattleEffectContext> _appliedEffect = new();
        private readonly ReactiveProperty<Vector2> _direction = new();
        private readonly ReactiveProperty<Vector2> _lookAt = new();
        private readonly HashSet<ushort> _hitHistory = new();

        private readonly Transform _pawn;
        private readonly Transform _muzzle;
        private readonly Collider2D _collider;
        private bool _isDisposed;

        public ushort Uid { get; private set; }
        public int Id { get; private set; }
        public ActorId ActorId { get; private set; }
        public RarityType Rarity { get; private set; }
        public StatSet Stat { get; }
        public GaugeSet Gauge { get; }
        public Vector2 PrePosition { get; private set; }
        public Vector2 Position => _pawn.position;
        public Vector2 MuzzlePosition => _muzzle.position;
        public Vector2 ColliderSize => _collider.bounds.size;

        public StateTag State => _state.CurrentValue;
        public ReadOnlyReactiveProperty<BattleEffectContext> AppliedEffect => _appliedEffect;
        public ReadOnlyReactiveProperty<Vector2> Direction => _direction;
        public ReadOnlyReactiveProperty<Vector2> LookAt => _lookAt;

        public BattleSystem(
            Transform pawn,
            Transform muzzle,
            Collider2D collider,
            ReadOnlyReactiveProperty<StateTag> state,
            IPublisher<BattleCueMessage> cuePublisher)
        {
            Stat = new StatSet();
            Gauge = new GaugeSet(Stat);
            _pawn = pawn;
            _muzzle = muzzle;
            _collider = collider;
            _state = state;
            _cuePublisher = cuePublisher;
        }

        public bool ContainsEffectTag(BattleEffectTag tag) => _effectTags.Contains(tag);

        public void Initialize(
            ushort uid,
            ActorId actorId,
            int id,
            RarityType rarity,
            Vector2 position)
        {
            Uid = uid;
            ActorId = actorId;
            Id = id;
            Rarity = rarity;
            PrePosition = position;
            _pawn.position = position;
            Reset();
        }

        public void Reset()
        {
            _hitHistory.Clear();
            _abilitySpecs.Clear();

            foreach (var effect in _effectSpecs)
            {
                effect.Deactivate(true);
            }

            _effectTags.Clear();
            _deactivateEffectSpecs.Clear();
            _effectSpecs.Clear();
            _statModifiers.Clear();
            _gaugeModifiers.Clear();
            Stat.Clear();
            Gauge.Clear();
        }

        public void SetPosition(Vector2 position)
        {
            PrePosition = _pawn.position;
            _pawn.position = position;
        }

        public void SetDirection(Vector2 direction)
        {
            _direction.Value = direction;
        }

        public void SetLookAt(Vector2 lookAt)
        {
            _lookAt.Value = lookAt;
        }

        public void GrantAbility(BattleAbility ability)
        {
            _abilitySpecs.Add(ability.Id, new BattleAbilitySpec(ability, Stat, _cuePublisher));
        }

        public bool CanActivateAbility(BattleAbilityId abilityId)
        {
            return _abilitySpecs.TryGetValue(abilityId, out var spec) && spec.CanActivateAbility(this);
        }

        public bool TryActivateAbility(ref BattleAbilityContext context) => _abilitySpecs.TryGetValue(context.AbilityId, out var spec) && spec.TryActivate(ref context);

        public void ActivateEffect(BattleEffectSpec spec)
        {
            if (!spec.TryActivate()) return;

            EnqueueModifiers(spec.StatModifiers, spec.GaugeModifiers);
            AddTags(spec.GrantedTags);
            _effectSpecs.Add(spec);
            _appliedEffect.Value = spec.Context;
            _appliedEffect.ForceNotify();
        }

        public void DeactivateEffect(BattleEffectId effectId)
        {
            foreach (var spec in _effectSpecs)
            {
                if (spec.Id == effectId)
                {
                    _deactivateEffectSpecs.Enqueue(spec);
                    return;
                }
            }
        }

        public bool TryAddHistory(ushort uid) => _hitHistory.Add(uid);

        public void Tick(float deltaTime)
        {
            UpdateAbilities(deltaTime);
            UpdateEffects(deltaTime);
            UpdateAttributeSet();
        }

        public void Dispose()
        {
            if (_isDisposed) return;

            _isDisposed = true;
            _appliedEffect?.Dispose();
        }

        private void UpdateAttributeSet()
        {
            while (_statModifiers.Count > 0)
            {
                var modifier = _statModifiers.Dequeue();
                Stat.Apply(modifier.Id, modifier.Value, modifier.Channel);
            }

            while (_gaugeModifiers.Count > 0)
            {
                var modifier = _gaugeModifiers.Dequeue();
                Gauge.Apply(modifier.Id, modifier.Value, modifier.Channel);
            }
        }

        private void UpdateEffects(float deltaTime)
        {
            DeactivateEffects();

            foreach (var spec in _effectSpecs)
            {
                if (spec.TryActivateForPeriodic(deltaTime))
                {
                    EnqueueModifiers(spec.StatPeriodicModifiers, spec.GaugePeriodicModifiers);
                }

                if (!spec.Tick(deltaTime))
                {
                    _deactivateEffectSpecs.Enqueue(spec);
                }
            }

            DeactivateEffects();
        }

        private void EnqueueModifiers(IReadOnlyList<BattleStatModifier> stats, IReadOnlyList<BattleGaugeModifier> gauges)
        {
            foreach (var stat in stats)
            {
                _statModifiers.Enqueue(stat);
            }

            foreach (var gauge in gauges)
            {
                _gaugeModifiers.Enqueue(gauge);
            }
        }

        private void UpdateAbilities(float deltaTime)
        {
            foreach (var abilitySpec in _abilitySpecs.Values)
            {
                abilitySpec.Tick(deltaTime);
            }
        }

        private void AddTags(IReadOnlyCollection<BattleEffectTag> tags)
        {
            foreach (var tag in tags)
            {
                _effectTags.Add(tag);
            }
        }

        private void RemoveTags(IReadOnlyCollection<BattleEffectTag> tags)
        {
            foreach (var tag in tags)
            {
                _effectTags.Remove(tag);
            }
        }

        private void DeactivateEffects()
        {
            while (_deactivateEffectSpecs.Count > 0)
            {
                var spec = _deactivateEffectSpecs.Dequeue();
                RemoveTags(spec.GrantedTags);
                spec.Deactivate();
                _effectSpecs.Remove(spec);
            }
        }
    }
}