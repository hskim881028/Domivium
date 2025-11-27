using System;
using Domivium.Client.Core.Component;
using Domivium.Client.Core.Component.Stat;
using Domivium.Client.Core.Component.Text;
using Domivium.Client.Core.Utility;
using Domivium.Client.Data.Context;
using Domivium.Client.Data.Item;
using UnityEngine;

namespace Domivium.Client.Contents.UIComponents.ItemContainer
{
    public class ItemInformation : MonoBehaviour
    {
        [SerializeField] private ItemVisual _itemVisual;
        [SerializeField] private StringText _itemName;
        [SerializeField] private StringText _itemType;
        [SerializeField] private FloatStat _weight;
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

        public void Show(Sprite sprite, ItemEntity item, ItemContext context)
        {
            gameObject.SetActive(true);
            _itemVisual.Show(sprite, item.Count, item.IsStackable);

            _itemName.Value = Converter.GetItemName(item.Type, item.Id);
            _itemType.Value = Converter.GetItemTypeName(item.Type);

            _attack.Hide();
            _defense.Hide();
            _attackRange.Hide();
            _attackSpeed.Hide();
            _moveSpeed.Hide();
            _projectileSpeed.Hide();
            _projectileCapacity.Hide();
            _reloadSpeed.Hide();
            _criticalRate.Hide();
            _criticalDamage.Hide();

            _weight.Set(context.Weight * Constant.Percent);
            switch (item.Type)
            {
                case ItemType.Weapon:
                    _attack.Set(context.Attack);
                    _attackRange.Set(context.AttackRange * Constant.Percent);
                    _projectileCapacity.Set(context.ProjectileCapacity);
                    _attackSpeed.Set(context.AttackSpeed * Constant.Percent);
                    _reloadSpeed.Set(context.ReloadSpeed * Constant.Percent);
                    _criticalRate.Set(context.CriticalRate * Constant.Percent);
                    _criticalDamage.Set(context.CriticalDamage * Constant.Percent);
                    break;
                case ItemType.Projectile:
                    _attack.Set(context.Attack);
                    _projectileSpeed.Set(context.ProjectileSpeed * Constant.Percent);
                    _criticalRate.Set(context.CriticalRate * Constant.Percent);
                    _criticalDamage.Set(context.CriticalDamage * Constant.Percent);
                    break;
                case ItemType.Helmet:
                case ItemType.Necklace:
                case ItemType.Backpack:
                case ItemType.Armor:
                case ItemType.Ring:
                case ItemType.Food:
                case ItemType.Potion:
                    break;
                case ItemType.None:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}