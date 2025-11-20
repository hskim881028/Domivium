using Domivium.Client.Core.Component;
using UnityEngine;
using UnityEngine.UI;

namespace Domivium.Client.Contents.UIComponents.ItemContainer
{
    public class Loot : ItemContainer
    {
        private const int ScrollTriggerCount = 12;

        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private GameObject _blocker;
        [SerializeField] private IntLimitText _capacity;

        public override void Reset()
        {
            _scrollRect.verticalNormalizedPosition = 1;
            Unlock();
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

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Lock()
        {
            _blocker.SetActive(true);
        }

        public void Unlock()
        {
            _blocker.SetActive(false);
        }
    }
}