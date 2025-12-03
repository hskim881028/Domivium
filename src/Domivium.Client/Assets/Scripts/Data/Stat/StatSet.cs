using System;
using System.Collections.Generic;
using UnityEngine;

namespace Domivium.Client.Data.Stat
{
    public sealed class StatSet
    {
        private readonly Stat[] _stats = new Stat[StatId.StatCount];
        private readonly HashSet<Action>[] _onChanged = new HashSet<Action>[StatId.StatCount];

        public void Register(StatId id, int value)
        {
            _stats[id] = new Stat(GetDomain(id), value);
        }

        public void AddListener(StatId id, Action onChanged)
        {
            if (onChanged == null) return;

            _onChanged[id] ??= new HashSet<Action>();
            _onChanged[id].Add(onChanged);
            onChanged.Invoke();
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

        public int RawValue(StatId id)
        {
            ref var s = ref Ref(id);
            return s.Value();
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
                Ref(i).Reset();
            }
        }

        public static StatDomain GetDomain(StatId id)
        {
            if (id == StatId.Health ||
                id == StatId.Hunger ||
                id == StatId.Stamina ||
                id == StatId.Sanity ||
                id == StatId.Durability ||
                id == StatId.Weight ||
                id == StatId.WeightCapacity ||
                id == StatId.InventoryCapacity ||
                id == StatId.ProjectileCapacity ||
                id == StatId.Attack ||
                id == StatId.Defense ||
                id == StatId.Penetration)
            {
                return StatDomain.Value;
            }

            return StatDomain.Rate;
        }

        private ref Stat Ref(StatId id) => ref _stats[id];
    }
}