using Domivium.Client.Core.Component;
using Domivium.Client.Core.Component.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Domivium.Client.Contents.UIComponents.ItemContainer
{
    public class Inventory : ItemContainer
    {
        private const int ScrollTriggerCount = 16;

        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private IntLimitText _capacity;
        [SerializeField] private IntLimitText _weight;

        public override void Reset()
        {
            _scrollRect.verticalNormalizedPosition = 1;
        }

        public override void SetSlot(int slotCount, int index, Sprite sprite, int itemCount, bool isStackable)
        {
            base.SetSlot(slotCount, index, sprite, itemCount, isStackable);
            _capacity.Value = slotCount;
        }

        public override void ClearSlot(int slotCount, int index)
        {
            base.ClearSlot(slotCount, index);
            _capacity.Value = slotCount;
        }

        public void SetInventoryCapacity(int capacity)
        {
            _scrollRect.vertical = capacity > ScrollTriggerCount;
            _scrollRect.verticalNormalizedPosition = 1;
            foreach (var slot in _slots)
            {
                slot.gameObject.SetActive(slot.SlotInfo.Index < capacity);
            }

            _capacity.Limit = capacity;
        }
    }
}