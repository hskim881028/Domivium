using System.Collections.Generic;
using Domivium.Client.Core.Actors.Unit;
using Domivium.Client.Core.Message;
using Domivium.Client.Core.State;
using Domivium.Client.Data.Stat;
using MessagePipe;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Domivium.Client.Core.Battle
{
    public sealed class BattleSystem : IBattleSystem
    {
        private readonly ObservableHashSet<BattleTag> _tags = new();
        private readonly Dictionary<BattleAbilityId, BattleAbilitySpec> _abilitySpecs = new();
        private readonly List<BattleEffectSpec> _effectSpecs = new();
        private readonly Queue<BattleStatModifier> _statModifiers = new();
        private readonly Queue<BattleGaugeModifier> _gaugeModifiers = new();
        private readonly ReadOnlyReactiveProperty<StateTag> _state;
        private readonly IPublisher<BattleCueMessage> _cuePublisher;

        public UnitType Type { get; private set; }
        public StatSet Stat { get; }
        public GaugeSet Gauge { get; }
        public Transform Unit { get; }
        public Vector3 UnitPosition => Unit.position;
        public StateTag State => _state.CurrentValue;

        public BattleSystem(
            Transform unit,
            ReadOnlyReactiveProperty<StateTag> state,
            IPublisher<BattleCueMessage> cuePublisher)
        {
            Unit = unit;
            Stat = new StatSet();
            Gauge = new GaugeSet(Stat);
            _state = state;
            _cuePublisher = cuePublisher;
        }

        public bool Contains(BattleTag tag) => _tags.Contains(tag);

        public void Reset()
        {
            _abilitySpecs.Clear();

            foreach (var effect in _effectSpecs)
            {
                effect.Deactivate(true);
            }

            _effectSpecs.Clear();
            _statModifiers.Clear();
            _gaugeModifiers.Clear();
            Stat.Clear();
            Gauge.Clear();
        }

        public void SetType(UnitType type)
        {
            Type = type;
        }

        public void GrantAbility(BattleAbility ability)
        {
            _abilitySpecs.Add(ability.Id, new BattleAbilitySpec(ability, Stat, _cuePublisher));
        }

        public bool TryActivateAbility(BattleAbilityId id, ref BattleAbilityContext context)
        {
            return _abilitySpecs.TryGetValue(id, out var spec) && spec.TryActivate(ref context);
        }

        public void ActivateEffect(BattleEffectSpec spec)
        {
            if (!spec.TryActivate(this)) return;

            EnqueueModifiers(spec.StatModifiers, spec.GaugeModifiers);
            AddTags(spec.GrantedTags);
            _effectSpecs.Add(spec);
        }

        public void Tick(float deltaTime)
        {
            UpdateAbilities(deltaTime);
            UpdateEffects(deltaTime);
            UpdateAttributeSet();
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
            var originalCount = _effectSpecs.Count;
            var write = 0;
            for (var i = 0; i < originalCount; ++i)
            {
                var effect = _effectSpecs[i];
                if (effect.Tick(deltaTime))
                {
                    if (!effect.TryActivateForPeriodic(deltaTime)) continue;

                    EnqueueModifiers(effect.StatPeriodicModifiers, effect.GaugePeriodicModifiers);
                    _effectSpecs[write++] = effect;
                }
                else
                {
                    RemoveTags(effect.GrantedTags);
                    effect.Deactivate();
                }
            }

            var tail = _effectSpecs.Count - write;
            if (tail > 0)
            {
                _effectSpecs.RemoveRange(write, tail);
            }
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

        private void AddTags(IReadOnlyCollection<BattleTag> tags)
        {
            foreach (var tag in tags)
            {
                _tags.Add(tag);
            }
        }

        private void RemoveTags(IReadOnlyCollection<BattleTag> tags)
        {
            foreach (var tag in tags)
            {
                _tags.Remove(tag);
            }
        }
    }
}