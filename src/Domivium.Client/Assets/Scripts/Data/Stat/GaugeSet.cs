using System;

namespace Domivium.Client.Data.Stat
{
    public sealed class GaugeSet
    {
        private readonly StatSet _stats;
        private readonly int[] _current = new int[StatId.StatCount];
        private readonly Action[] _onChanged = new Action[StatId.StatCount];

        public GaugeSet(StatSet stats)
        {
            _stats = stats;
        }

        public int Current(StatId id) => _current[id];

        public int Max(StatId id) => _stats.Value(id);

        public void Register(StatId id, int value, Action onChanged = null)
        {
            Set(id, value);
            _onChanged[id] = onChanged;
            _onChanged[id]?.Invoke();
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

            _onChanged[id]?.Invoke();
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