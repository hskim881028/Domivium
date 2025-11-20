using System;
using Domivium.Client.Core.Component;
using Domivium.Client.Data.Context;
using Domivium.Client.Data.Item;
using TMPro;
using UnityEngine;

namespace Domivium.Client.Contents.UIComponents.ItemContainer
{
    public class ItemInformation : MonoBehaviour
    {
        [SerializeField] private ItemVisual _itemVisual;
        [SerializeField] private TextMeshProUGUI _itemNameText;
        [SerializeField] private TextMeshProUGUI _itemTypeText;

        [SerializeField] private IntLimitText _durationText;

        [SerializeField] private TextMeshProUGUI _weightText;

        [SerializeField] private TextMeshProUGUI _attackText;
        [SerializeField] private TextMeshProUGUI _defenseText;

        [SerializeField] private TextMeshProUGUI _attackRangeText;

        [SerializeField] private TextMeshProUGUI _projectileCapacityText;

        [SerializeField] private TextMeshProUGUI _attackSpeedText;
        [SerializeField] private TextMeshProUGUI _reloadSpeedText;
        [SerializeField] private TextMeshProUGUI _moveSpeedText;

        [SerializeField] private TextMeshProUGUI _criticalRateText;
        [SerializeField] private TextMeshProUGUI _criticalDamageText;

        [SerializeField] private GameObject _defense;
        [SerializeField] private GameObject _attackRange;
        [SerializeField] private GameObject _projectileCapacity;
        [SerializeField] private GameObject _attackSpeed;
        [SerializeField] private GameObject _reloadSpeed;
        [SerializeField] private GameObject _moveSpeed;


        public void Show(Sprite sprite, ItemData item, ItemContext context)
        {
            gameObject.SetActive(true);
            _itemVisual.Show(sprite, item.Count, item.IsStackable);

            _itemNameText.text = $"{item.Type}_{item.Id}";
            _itemTypeText.text = item.Type.ToString();
            _durationText.Limit = context.Durability;
            _durationText.Value = context.Durability;

            _weightText.text = $"{context.Weight}kg";

            _attackText.text = $"{context.Attack}";
            _defenseText.text = $"{context.Defense}";

            _attackRangeText.text = $"{context.AttackRange * 0.01f}m";

            _projectileCapacityText.text = $"{context.ProjectileCapacity}";

            _attackSpeedText.text = $"{context.AttackSpeed * 0.01f}";
            _reloadSpeedText.text = $"{context.ReloadSpeed * 0.01f}s";
            _moveSpeedText.text = $"{context.MoveSpeed * 0.01f}";

            _criticalRateText.text = $"{context.CriticalRate * 0.01f}%";
            _criticalDamageText.text = $"+{context.CriticalDamage * 0.01f}%";

            _defense.SetActive(false);
            switch (item.Type)
            {
                case ItemType.Weapon:
                    _attackRange.SetActive(true);
                    _projectileCapacity.SetActive(true);
                    _attackSpeed.SetActive(true);
                    _reloadSpeed.SetActive(true);
                    _moveSpeed.SetActive(false);
                    break;
                case ItemType.Projectile:
                    _attackRange.SetActive(false);
                    _projectileCapacity.SetActive(false);
                    _attackSpeed.SetActive(false);
                    _reloadSpeed.SetActive(false);
                    _moveSpeed.SetActive(true);
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