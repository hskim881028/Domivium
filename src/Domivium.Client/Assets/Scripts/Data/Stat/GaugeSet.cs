using System;
using System.Collections.Generic;

namespace Domivium.Client.Data.Stat
{
    public sealed class GaugeSet
    {
        private readonly StatSet _stats;
        private readonly int[] _current = new int[StatId.StatCount];
        private readonly HashSet<Action>[] _onChanged = new HashSet<Action>[StatId.StatCount];

        public GaugeSet(StatSet stats)
        {
            _stats = stats;
        }

        public int Current(StatId id) => _current[id];

        public int Max(StatId id) => _stats.Value(id);

        public void Register(StatId id, int value)
        {
            Set(id, value);
        }

        public void AddListener(StatId id, Action onChanged)
        {
            if (onChanged == null) return;

            _onChanged[id] ??= new HashSet<Action>();
            _onChanged[id].Add(onChanged);
            onChanged?.Invoke();
        }

        public void RemoveListener(StatId id, Action onChanged)
        {
            _onChanged[id].Remove(onChanged);
        }

        public void Apply(StatId id, int value, GaugeChannel channel)
        {
            if (channel == GaugeChannel.Set)
            {
                Set(id, value);
            }
            else if (channel == GaugeChannel.Add)
            {
                Set(id, Current(id) + value);
            }
            else if (channel == GaugeChannel.Max)
            {
                Set(id, Max(id));
            }
            else if (channel == GaugeChannel.Empty)
            {
                Set(id, 0);
            }

            if (_onChanged[id] == null) return;

            foreach (var action in _onChanged[id])
            {
                action?.Invoke();
            }
        }

        public void Clear()
        {
            for (var i = 0; i < StatId.StatCount; i++)
            {
                if (_onChanged[i] == null) continue;

                _onChanged[i] = null;
                Set(i, 0);
            }
        }

        private void Set(StatId id, int value)
        {
            if (value < 0)
            {
                value = 0;
            }

            var max = Max(id);
            if (value > max)
            {
                value = max;
            }

            _current[id] = value;
        }
    }
}