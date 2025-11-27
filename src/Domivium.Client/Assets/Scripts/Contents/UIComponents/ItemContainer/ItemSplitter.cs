using System;
using Domivium.Client.Core.Component;
using Domivium.Client.Core.Component.Text;
using Domivium.Client.Data.Item;
using UnityEngine;
using UnityEngine.UI;

namespace Domivium.Client.Contents.UIComponents.ItemContainer
{
    public class ItemSplitter : MonoBehaviour
    {
        [SerializeField] private IntLimitText _countText;
        [SerializeField] private Slider _slider;
        [SerializeField] private Button _confirmButton;
        [SerializeField] private Button _cancelButton;

        private ItemSlotEntry _slot;

        public Action<ItemSlotEntry, int> Split { get; set; }

        private void Awake()
        {
            _confirmButton.onClick.AddListener(Confirm);
            _cancelButton.onClick.AddListener(Hide);
            _slider.onValueChanged.AddListener(OnValueChanged);
        }

        public void Show(ItemSlotEntry slot, int itemCount)
        {
            _slot = slot;
            _slider.minValue = 0;
            _slider.maxValue = itemCount;
            _slider.value = 0;
            _countText.Value = 0;
            _countText.Limit = itemCount;
            _confirmButton.interactable = false;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            _slot = ItemSlotEntry.Default;
        }

        private void Confirm()
        {
            Split?.Invoke(_slot, (int)_slider.value);
            Hide();
        }

        private void OnValueChanged(float value)
        {
            var v = (int)value;
            _slider.value = v;
            _countText.Value = v;
            _confirmButton.interactable = v != 0 && v != _countText.Limit;
        }
    }
}