using System;
using Domivium.Client.Data.Item;
using UnityEngine;
using UnityEngine.UI;

namespace Domivium.Client.Contents.UIComponents.ItemContainer
{
    public class ItemSelector : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private Button _equipButton;
        [SerializeField] private Button _unequipButton;
        [SerializeField] private Button _splitButton;
        [SerializeField] private Button _useButton;
        [SerializeField] private Button _keepButton;
        [SerializeField] private Button _takeOutButton;
        [SerializeField] private Button _removeButton;
        [SerializeField] private RectTransform _pivot;

        private ItemSlotEntry _slot;

        public Action<ItemSlotEntry> Equip { get; set; }
        public Action<ItemSlotEntry> Unequip { get; set; }
        public Action<ItemSlotEntry> Split { get; set; }
        public Action<ItemSlotEntry> Use { get; set; }
        public Action<ItemSlotEntry> Keep { get; set; }
        public Action<ItemSlotEntry> TakeOut { get; set; }
        public Action<ItemSlotEntry> Remove { get; set; }

        private void Awake()
        {
            _equipButton.onClick.AddListener(() => Equip?.Invoke(_slot));
            _unequipButton.onClick.AddListener(() => Unequip?.Invoke(_slot));
            _splitButton.onClick.AddListener(() => Split?.Invoke(_slot));
            _useButton.onClick.AddListener(() => Use?.Invoke(_slot));
            _keepButton.onClick.AddListener(() => Keep?.Invoke(_slot));
            _takeOutButton.onClick.AddListener(() => TakeOut?.Invoke(_slot));
            _removeButton.onClick.AddListener(() => Remove?.Invoke(_slot));
        }

        public void Show(
            RectTransform parent,
            ItemSlotEntry slot,
            ItemType itemType,
            bool canKeep)
        {
            switch (slot.Type)
            {
                case ItemSlotType.Equipment:
                    _unequipButton.gameObject.SetActive(true);
                    _equipButton.gameObject.SetActive(false);
                    _splitButton.gameObject.SetActive(false);
                    _useButton.gameObject.SetActive(false);
                    _keepButton.gameObject.SetActive(false);
                    _takeOutButton.gameObject.SetActive(false);
                    _removeButton.gameObject.SetActive(false);
                    break;
                case ItemSlotType.Inventory:
                    _keepButton.gameObject.SetActive(canKeep);
                    _equipButton.gameObject.SetActive(true);
                    _unequipButton.gameObject.SetActive(false);
                    _takeOutButton.gameObject.SetActive(false);
                    switch (itemType)
                    {
                        case ItemType.Weapon:
                        case ItemType.Necklace:
                        case ItemType.Ring:
                        case ItemType.Body:
                        case ItemType.Feet:
                        case ItemType.Head:
                        case ItemType.Bag:
                            _splitButton.gameObject.SetActive(false);
                            _useButton.gameObject.SetActive(false);
                            break;
                        case ItemType.Projectile:
                        case ItemType.Cash:
                        case ItemType.Material:
                            _splitButton.gameObject.SetActive(true);
                            _useButton.gameObject.SetActive(false);
                            break;
                        case ItemType.Potion:
                        case ItemType.Food:
                            _splitButton.gameObject.SetActive(true);
                            _useButton.gameObject.SetActive(true);
                            break;
                        case ItemType.None:
                        default:
                            throw new ArgumentOutOfRangeException(nameof(itemType), itemType, null);
                    }
                    _removeButton.gameObject.SetActive(true);
                    break;
                case ItemSlotType.Loot:
                    _takeOutButton.gameObject.SetActive(true);
                    _equipButton.gameObject.SetActive(false);
                    _unequipButton.gameObject.SetActive(false);
                    switch (itemType)
                    {
                        case ItemType.Weapon:
                        case ItemType.Necklace:
                        case ItemType.Ring:
                        case ItemType.Head:
                        case ItemType.Body:
                        case ItemType.Feet:
                        case ItemType.Bag:
                            _splitButton.gameObject.SetActive(false);
                            break;
                        case ItemType.Projectile:
                        case ItemType.Potion:
                        case ItemType.Food:
                        case ItemType.Cash:
                        case ItemType.Material:
                            _splitButton.gameObject.SetActive(true);
                            break;
                        case ItemType.None:
                        default:
                            throw new ArgumentOutOfRangeException(nameof(itemType), itemType, null);
                    }
                    _useButton.gameObject.SetActive(false);
                    _keepButton.gameObject.SetActive(false);
                    _removeButton.gameObject.SetActive(true);
                    break;
                case ItemSlotType.None:
                default:
                    throw new ArgumentOutOfRangeException(nameof(slot.Type), slot.Type, null);
            }

            _pivot.SetParent(parent);
            _pivot.localPosition = Vector3.zero;
            _pivot.localRotation = Quaternion.identity;
            _pivot.localScale = Vector3.one;
            _rectTransform.position = _pivot.position;
            _pivot.gameObject.SetActive(true);
            _rectTransform.gameObject.SetActive(true);
            _slot = slot;
        }

        public void Hide()
        {
            _slot = ItemSlotEntry.Default;
            _pivot.gameObject.SetActive(false);
            _rectTransform.gameObject.SetActive(false);
        }

        private void Update()
        {
            _rectTransform.position = _pivot.position;
        }
    }
}