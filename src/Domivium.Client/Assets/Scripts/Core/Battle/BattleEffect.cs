using System.Collections.Generic;
using System.Linq;
using Domivium.Client.Data.Stat;

namespace Domivium.Client.Core.Battle
{
    public abstract class BattleEffect
    {
        private readonly List<BattleStatModifier> _statModifiers = new();
        private readonly List<BattleStatModifier> _statPeriodicModifiers = new();
        private readonly List<BattleGaugeModifier> _gaugeModifiers = new();
        private readonly List<BattleGaugeModifier> _gaugePeriodicModifiers = new();

        protected abstract IReadOnlyCollection<BattleTag> RequiredTags { get; }
        protected abstract IReadOnlyCollection<BattleTag> BlockedTags { get; }
        public abstract BattleEffectId Id { get; }
        public abstract float Duration { get; }
        public abstract float PeriodicInterval { get; }
        public abstract BattleCueId CueId { get; }
        public abstract BattleCueId PeriodicCueId { get; }
        public abstract BattleCueId DeactivateCueId { get; }

        public BattleContext Context { get; private set; }
        public abstract IReadOnlyCollection<BattleTag> GrantedTags { get; }
        public IReadOnlyList<BattleStatModifier> StatModifiers => _statModifiers;
        public IReadOnlyList<BattleStatModifier> StatPeriodicModifiers => _statPeriodicModifiers;
        public IReadOnlyList<BattleGaugeModifier> GaugeModifiers => _gaugeModifiers;
        public IReadOnlyList<BattleGaugeModifier> GaugePeriodicModifiers => _gaugePeriodicModifiers;

        protected BattleEffect(BattleContext context)
        {
            Context = context;
        }

        public abstract void Activate(BattleSystem owner);

        public void Reset(BattleContext context)
        {
            Context = context;
            _statModifiers.Clear();
            _statPeriodicModifiers.Clear();
            _gaugeModifiers.Clear();
            _gaugePeriodicModifiers.Clear();
        }

        public bool PassesTagRequirements(IReadOnlyCollection<BattleTag> tags)
            => RequiredTags.All(tags.Contains) && BlockedTags.All(t => !tags.Contains(t));

        protected void AddStatModifier(StatId id, int value, StatChannel channel)
        {
            _statModifiers.Add(new BattleStatModifier(id, value, channel));
        }

        protected void AddStatPeriodicModifier(StatId id, int value, StatChannel channel)
        {
            _statPeriodicModifiers.Add(new BattleStatModifier(id, value, channel));
        }

        protected void AddGaugeModifier(StatId id, int value, GaugeChannel channel)
        {
            _gaugeModifiers.Add(new BattleGaugeModifier(id, value, channel));
        }

        protected void AddGaugePeriodicModifier(StatId id, int value, GaugeChannel channel)
        {
            _gaugePeriodicModifiers.Add(new BattleGaugeModifier(id, value, channel));
        }
    }
}