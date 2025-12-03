using System.Collections.Generic;
using Domivium.Client.Core.Component.Stat;
using Domivium.Client.Data.Stat;
using UnityEngine;

namespace Domivium.Client.Contents.UIComponents.ItemContainer
{
    public class Status : MonoBehaviour
    {
        [SerializeField] private IntStat _level;
        [SerializeField] private SliderStat _exp;

        [SerializeField] private SliderStat _health;
        [SerializeField] private SliderStat _hunger;
        [SerializeField] private SliderStat _stamina;
        [SerializeField] private SliderStat _sanity;

        [SerializeField] private FloatStat _attack;
        [SerializeField] private FloatStat _defense;
        [SerializeField] private FloatStat _penetration;
        [SerializeField] private FloatStat _moveSpeed;
        [SerializeField] private FloatStat _attackRange;
        [SerializeField] private FloatStat _attackSpeed;
        [SerializeField] private FloatStat _projectileSpeed;
        [SerializeField] private FloatStat _projectileCapacity;
        [SerializeField] private FloatStat _reloadSpeed;
        [SerializeField] private FloatStat _criticalRate;
        [SerializeField] private FloatStat _criticalDamage;

        private IReadOnlyDictionary<StatId, SliderStat> _gauges;
        private IReadOnlyDictionary<StatId, FloatStat> _stats;

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void SetLevel(int value)
        {
            _level.Set(value);
        }

        public void SetFilledExperience(int value)
        {
            _exp.SetCurrent(value);
        }

        public void SetExperience(int value)
        {
            _exp.SetLimit(value);
        }

        public void SetStat(StatId statId, float value)
        {
            _stats ??= new Dictionary<StatId, FloatStat>
            {
                { StatId.Attack, _attack },
                { StatId.Defense, _defense },
                { StatId.Penetration, _penetration },
                { StatId.MoveSpeed, _moveSpeed },
                { StatId.AttackRange, _attackRange },
                { StatId.AttackSpeed, _attackSpeed },
                { StatId.ProjectileSpeed, _projectileSpeed },
                { StatId.ProjectileCapacity, _projectileCapacity },
                { StatId.ReloadSpeed, _reloadSpeed },
                { StatId.CriticalRate, _criticalRate },
                { StatId.CriticalDamage, _criticalDamage }
            };

            _stats[statId].Set(value);
        }

        public void SetGauge(StatId statId, int current, int limit)
        {
            _gauges ??= new Dictionary<StatId, SliderStat>
            {
                { StatId.Health, _health },
                { StatId.Hunger, _hunger },
                { StatId.Stamina, _stamina },
                { StatId.Sanity, _sanity }
            };

            _gauges[statId].Set(current, limit);
        }
    }
}