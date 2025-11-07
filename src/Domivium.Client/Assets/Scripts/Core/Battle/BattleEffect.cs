using System.Collections.Generic;
using System.Linq;
using Domivium.Client.Core.State;
using Domivium.Client.Core.Utility;
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

        public abstract BattleEffectId Id { get; }
        protected virtual IReadOnlyCollection<BattleTag> RequiredBattleTags => TagGenerator.EmptyBattleTag;
        protected virtual IReadOnlyCollection<BattleTag> BlockedBattleTags => TagGenerator.EmptyBattleTag;
        protected virtual IReadOnlyCollection<StateTag> BlockedStateTags => TagGenerator.DefaultBlockedStateTag;
        public virtual IReadOnlyCollection<BattleTag> GrantedBattleTags => TagGenerator.EmptyBattleTag;
        public virtual BattleCueId CueId => BattleCueId.None;
        public virtual BattleCueId PeriodicCueId => BattleCueId.None;
        public virtual BattleCueId DeactivateCueId => BattleCueId.None;
        public ref BattleEffectContext Context => ref _context;
        public IReadOnlyList<BattleStatModifier> StatModifiers => _statModifiers;
        public IReadOnlyList<BattleStatModifier> StatPeriodicModifiers => _statPeriodicModifiers;
        public IReadOnlyList<BattleGaugeModifier> GaugeModifiers => _gaugeModifiers;
        public IReadOnlyList<BattleGaugeModifier> GaugePeriodicModifiers => _gaugePeriodicModifiers;
        public float Duration { get; protected set; }
        public float PeriodicInterval { get; protected set; }

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

        public bool TryActivate() => PassesTagRequirements() && OnActivate();

        protected abstract bool OnActivate();

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

        private bool PassesTagRequirements()
        {
            if (BlockedStateTags.Contains(Context.Owner.State))
            {
                return false;
            }

            foreach (var tag in BlockedBattleTags)
            {
                if (Context.Owner.ContainsTag(tag)) return false;
            }

            foreach (var tag in RequiredBattleTags)
            {
                if (!Context.Owner.ContainsTag(tag)) return false;
            }

            return true;
        }
    }
}