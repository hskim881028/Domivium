using System;
using UnityEngine;

namespace Domivium.Client.Data.Stat
{
    public sealed class StatSet
    {
        private readonly Stat[] _stats = new Stat[StatId.StatCount];
        private readonly Action[] _onChanged = new Action[StatId.StatCount];

        public void Register(StatId id, int value, Action onChanged = null)
        {
            _stats[id] = new Stat(GetDomain(id), value);
            _onChanged[id] = onChanged;
            _onChanged[id]?.Invoke();
        }

        public int Value(StatId id)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            ref var s = ref Ref(id);
            if (s.Domain != StatDomain.Value)
            {
                Debug.LogException(new InvalidOperationException($"Domain mismatch: {id} (expected {StatDomain.Value}, actual={s.Domain})"));
            }

            return s.Value();
#else
            return Ref(id).Value();
#endif
        }

        public float RateValue(StatId id)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            ref var s = ref Ref(id);
            if (s.Domain != StatDomain.Rate)
            {
                Debug.LogException(new InvalidOperationException($"Domain mismatch: {id} (expected {StatDomain.Rate}, actual={s.Domain})"));
            }

            return s.Value() * Constant.Percent;
#else
            return Ref(id).Value() * Constant.Percent;
#endif
        }

        public void Apply(StatId id, int value, StatChannel channel)
        {
            if (channel == StatChannel.Add)
            {
                Ref(id).Add(value);
            }
            else if (channel == StatChannel.PostAdd)
            {
                Ref(id).PostAdd(value);
            }
            else if (channel == StatChannel.AddMultiplier)
            {
                Ref(id).AddMultiplier(value);
            }

            _onChanged[id]?.Invoke();
        }

        public void Reset(StatId id)
        {
            Ref(id).Reset();
            _onChanged[id]?.Invoke();
        }

        public void Clear()
        {
            for (var i = 0; i < StatId.StatCount; i++)
            {
                _onChanged[i] = null;
                Reset(i);
            }
        }

        private ref Stat Ref(StatId id) => ref _stats[id];

        private static StatDomain GetDomain(StatId id)
        {
            if (id == StatId.Health ||
                id == StatId.Attack ||
                id == StatId.Defense)
            {
                return StatDomain.Value;
            }

            return StatDomain.Rate;
        }
    }
}