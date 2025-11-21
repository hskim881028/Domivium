using Domivium.Client.Core.Component.Stat;
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
        [SerializeField] private FloatStat _projectileMoveSpeed;
        [SerializeField] private FloatStat _projectileCapacity;
        [SerializeField] private FloatStat _reloadSpeed;
        [SerializeField] private FloatStat _criticalRate;
        [SerializeField] private FloatStat _criticalDamage;
    }
}