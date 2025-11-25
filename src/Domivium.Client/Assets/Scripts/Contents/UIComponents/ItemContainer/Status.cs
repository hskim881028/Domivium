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
        [SerializeField] private FloatStat _moveSpeed;
        [SerializeField] private FloatStat _attackRange;
        [SerializeField] private FloatStat _attackSpeed;
        [SerializeField] private FloatStat _projectileSpeed;
        [SerializeField] private FloatStat _projectileCapacity;
        [SerializeField] private FloatStat _reloadSpeed;
        [SerializeField] private FloatStat _criticalRate;
        [SerializeField] private FloatStat _criticalDamage;

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

        public void SetExp(int current, int limit)
        {
            _exp.Set(current, limit);
        }


        public void SetStat(StatId statId, float value)
        {
            if (statId == StatId.Attack)
            {
                _attack.Set(value);
            }
            else if (statId == StatId.Defense)
            {
                _defense.Set(value);
            }
            else if (statId == StatId.MoveSpeed)
            {
                _moveSpeed.Set(value);
            }
            else if (statId == StatId.AttackRange)
            {
                _attackRange.Set(value);
            }
            else if (statId == StatId.AttackSpeed)
            {
                _attackSpeed.Set(value);
            }
            else if (statId == StatId.ProjectileSpeed)
            {
                _projectileSpeed.Set(value);
            }
            else if (statId == StatId.ProjectileCapacity)
            {
                _projectileCapacity.Set(value);
            }
            else if (statId == StatId.ReloadSpeed)
            {
                _reloadSpeed.Set(value);
            }
            else if (statId == StatId.CriticalRate)
            {
                _criticalRate.Set(value);
            }
            else if (statId == StatId.CriticalDamage)
            {
                _criticalDamage.Set(value);
            }
        }

        public void SetGauge(StatId statId, int current, int limit)
        {
            if (statId == StatId.Health)
            {
                _health.Set(current, limit);
            }
            else if (statId == StatId.Hunger)
            {
                _hunger.Set(current, limit);
            }
            else if (statId == StatId.Stamina)
            {
                _stamina.Set(current, limit);
            }
            else if (statId == StatId.Sanity)
            {
                _sanity.Set(current, limit);
            }
        }
    }
}