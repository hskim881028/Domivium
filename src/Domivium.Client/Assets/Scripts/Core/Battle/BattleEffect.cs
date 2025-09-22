using System.Collections.Generic;
using System.Linq;
using Domivium.Client.Core.State;
using Domivium.Client.Data.Stat;

namespace Domivium.Client.Core.Battle
{
    public abstract class BattleEffect
    {
        private readonly List<BattleStatModifier> _statModifiers = new();
        private readonly List<BattleStatModifier> _statPeriodicModifiers = new();
        private readonly List<BattleGaugeModifier> _gaugeModifiers = new();
        private readonly List<BattleGaugeModifier> _gaugePeriodicModifiers = new();
        private BattleEffectContext _context;

        protected abstract IReadOnlyCollection<BattleTag> RequiredBattleTags { get; }
        protected abstract IReadOnlyCollection<BattleTag> BlockedBattleTags { get; }
        protected abstract IReadOnlyCollection<StateTag> BlockedStateTags { get; }
        public abstract BattleEffectId Id { get; }
        public abstract float Duration { get; }
        public abstract float PeriodicInterval { get; }
        public abstract BattleCueId CueId { get; }
        public abstract BattleCueId PeriodicCueId { get; }
        public abstract BattleCueId DeactivateCueId { get; }

        public ref BattleEffectContext Context => ref _context;
        public abstract IReadOnlyCollection<BattleTag> GrantedBattleTags { get; }
        public IReadOnlyList<BattleStatModifier> StatModifiers => _statModifiers;
        public IReadOnlyList<BattleStatModifier> StatPeriodicModifiers => _statPeriodicModifiers;
        public IReadOnlyList<BattleGaugeModifier> GaugeModifiers => _gaugeModifiers;
        public IReadOnlyList<BattleGaugeModifier> GaugePeriodicModifiers => _gaugePeriodicModifiers;

        protected BattleEffect(ref BattleEffectContext context)
        {
            _context = context;
        }

        public void Reset(BattleEffectContext context)
        {
            _context = context;
            _statModifiers.Clear();
            _statPeriodicModifiers.Clear();
            _gaugeModifiers.Clear();
            _gaugePeriodicModifiers.Clear();
        }

        public bool TryActivate(BattleSystem source) => PassesTagRequirements(source) && OnActivate(source);

        protected abstract bool OnActivate(BattleSystem source);

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

        private bool PassesTagRequirements(BattleSystem source)
        {
            if (BlockedStateTags.Contains(source.State))
            {
                return false;
            }

            foreach (var tag in BlockedBattleTags)
            {
                if (source.Contains(tag)) return false;
            }

            foreach (var tag in RequiredBattleTags)
            {
                if (!source.Contains(tag)) return false;
            }

            return true;
        }
    }
}