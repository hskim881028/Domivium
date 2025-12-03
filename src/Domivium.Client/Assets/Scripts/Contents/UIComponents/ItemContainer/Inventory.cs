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

        public override void Reset()
        {
            _scrollRect.verticalNormalizedPosition = 1;
        }

        public void SetFilledCapacity(int capacity)
        {
            _capacity.Value = capacity;
        }

        public void SetCapacity(int capacity)
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