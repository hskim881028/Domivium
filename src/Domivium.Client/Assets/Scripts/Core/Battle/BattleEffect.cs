using System;
using System.Collections.Generic;
using System.Linq;
using Domivium.Client.Core.State;
using Domivium.Client.Core.Systems;
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
        private readonly List<Action> _actionModifiers = new();
        private readonly List<Action> _actionPeriodicModifiers = new();

        private BattleEffectContext _context;

        public abstract BattleEffectId Id { get; }
        public virtual BattleCueId CueId => BattleCueId.None;
        public virtual BattleCueId PeriodicCueId => BattleCueId.None;
        public virtual BattleCueId DeactivateCueId => BattleCueId.None;
        public ref BattleEffectContext Context => ref _context;
        public IReadOnlyList<BattleStatModifier> StatModifiers => _statModifiers;
        public IReadOnlyList<BattleStatModifier> StatPeriodicModifiers => _statPeriodicModifiers;
        public IReadOnlyList<BattleGaugeModifier> GaugeModifiers => _gaugeModifiers;
        public IReadOnlyList<BattleGaugeModifier> GaugePeriodicModifiers => _gaugePeriodicModifiers;
        public IReadOnlyList<Action> ActionModifiers => _actionModifiers;
        public IReadOnlyList<Action> ActionPeriodicModifiers => _actionPeriodicModifiers;
        public float Duration { get; protected set; }
        public float PeriodicInterval { get; protected set; }

        public virtual IReadOnlyCollection<BattleEffectTag> GrantedEffectTags => TagGenerator.EmptyBattleEffectTag;
        protected virtual IReadOnlyCollection<BattleEffectTag> RequiredEffectTags => TagGenerator.EmptyBattleEffectTag;
        protected virtual IReadOnlyCollection<BattleEffectTag> BlockedEffectTags => TagGenerator.EmptyBattleEffectTag;
        protected virtual IReadOnlyCollection<StateTag> BlockedStateTags => TagGenerator.DefaultBlockedStateTag;
        protected readonly IItemUsageSystemCommand ItemUsage;

        protected BattleEffect(IItemUsageSystemCommand itemUsage, ref BattleEffectContext context)
        {
            ItemUsage = itemUsage;
            _context = context;
        }

        public virtual void Reset(BattleEffectContext context)
        {
            _context = context;
            _statModifiers.Clear();
            _statPeriodicModifiers.Clear();
            _gaugeModifiers.Clear();
            _gaugePeriodicModifiers.Clear();
            _actionModifiers.Clear();
            _actionPeriodicModifiers.Clear();
            Duration = 0;
            PeriodicInterval = 0;
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

        protected void AddActionModifier(Action action)
        {
            _actionModifiers.Add(action);
        }

        protected void AddActionPeriodicModifier(Action action)
        {
            _actionPeriodicModifiers.Add(action);
        }

        private bool PassesTagRequirements()
        {
            if (BlockedStateTags.Contains(Context.Owner.State))
            {
                return false;
            }

            foreach (var tag in BlockedEffectTags)
            {
                if (Context.Owner.ContainsEffectTag(tag)) return false;
            }

            foreach (var tag in RequiredEffectTags)
            {
                if (!Context.Owner.ContainsEffectTag(tag)) return false;
            }

            return true;
        }
    }
}