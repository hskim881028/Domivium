using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Domivium.Client.Core.Actors;
using Domivium.Client.Data.Stat;
using UnityEngine;

namespace Domivium.Client.Core.Battle
{
    public sealed class BattleSystem
    {
        private readonly Dictionary<BattleAbilityId, BattleAbilitySpec> _abilitySpecs = new();
        private readonly List<BattleEffectSpec> _effectSpecs = new();
        private readonly Queue<BattleStatModifier> _statModifiers = new();
        private readonly Queue<BattleGaugeModifier> _gaugeModifiers = new();
        private readonly TagSet _tagSet;

        public IReadOnlyCollection<ActorTag> Tags => _tagSet.Tags;
        public Transform Unit { get; }
        public StatSet Stat { get; }
        public GaugeSet Gauge { get; }

        public BattleSystem(TagSet tagSet, Transform unit)
        {
            _tagSet = tagSet;
            Unit = unit;
            Stat = new StatSet();
            Gauge = new GaugeSet(Stat);
        }

        public BattleContext CreateBattleContext(BattleAbility ability) => new() { Source = this, Unit = Unit, Ability = ability };

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

        public void GrantAbility(BattleAbilitySpec ability)
        {
            _abilitySpecs.Add(ability.Id, ability);
        }

        public bool TryActivateAbility(BattleAbilityId id) => _abilitySpecs.TryGetValue(id, out var spec) && spec.TryActivate(this);

        public void ActivateEffect(BattleEffectSpec spec)
        {
            if (!spec.TryActivate(this)) return;

            EnqueueModifiers(spec.StatModifiers, spec.GaugeModifiers);
            AddTags(spec.GrantedTags);
            _effectSpecs.Add(spec);
        }

        public void Tick(float deltaTime)
        {
            if (_tagSet.Contains(ActorTag.Die)) return;

            UpdateAbilities(deltaTime);
            UpdateEffects(deltaTime);
            UpdateAttributeSet();

            if (Gauge.Current(StatId.Health) < 150)
            {
                _tagSet.Add(ActorTag.Die);
                Test().Forget();
            }
        }

        private async UniTaskVoid Test()
        {
            await Awaitable.WaitForSecondsAsync(1);
            _tagSet.Add(ActorTag.Despawn);
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

        private void AddTags(IReadOnlyCollection<ActorTag> tags)
        {
            foreach (var tag in tags)
            {
                _tagSet.Add(tag);
            }
        }

        private void RemoveTags(IReadOnlyCollection<ActorTag> tags)
        {
            foreach (var tag in tags)
            {
                _tagSet.Remove(tag);
            }
        }
    }
}