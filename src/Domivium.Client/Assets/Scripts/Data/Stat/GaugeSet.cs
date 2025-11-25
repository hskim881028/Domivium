using System;
using System.Collections.Generic;

namespace Domivium.Client.Data.Stat
{
    public sealed class GaugeSet
    {
        private readonly int[] _current = new int[StatId.StatCount];
        private readonly int[] _max = new int[StatId.StatCount];
        private readonly HashSet<Action>[] _onChanged = new HashSet<Action>[StatId.StatCount];

        public int Current(StatId id) => _current[id];

        public int Max(StatId id) => _max[id];

        public void Register(StatId id, int value, int max)
        {
            _current[id] = value;
            _max[id] = max;
        }

        public void AddListener(StatId id, Action onChanged)
        {
            if (onChanged == null) return;

            _onChanged[id] ??= new HashSet<Action>();
            _onChanged[id].Add(onChanged);
            onChanged.Invoke();
        }

        public void ApplyMax(StatId id, int value, GaugeChannel channel)
        {
            if (channel == GaugeChannel.Set)
            {
                _max[id] = Math.Max(0, value);
            }
            else if (channel == GaugeChannel.Add)
            {
                _max[id] = Math.Max(0, _max[id] + value);
            }

            if (_onChanged[id] == null) return;

            foreach (var action in _onChanged[id])
            {
                action?.Invoke();
            }
        }

        public void Apply(StatId id, int value, GaugeChannel channel)
        {
            if (channel == GaugeChannel.Set)
            {
                Set(id, value, true);
            }
            else if (channel == GaugeChannel.Add)
            {
                Set(id, Current(id) + value);
            }
            else if (channel == GaugeChannel.Max)
            {
                Set(id, Max(id));
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

        private void Set(StatId id, int value, bool allowExceedMax = false)
        {
            if (value < 0)
            {
                value = 0;
            }

            if (!allowExceedMax)
            {
                var max = Max(id);
                if (value > max)
                {
                    value = max;
                }
            }

            _current[id] = value;
        }
    }
}