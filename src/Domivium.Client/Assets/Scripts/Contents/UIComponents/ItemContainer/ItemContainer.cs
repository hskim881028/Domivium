using System.Collections.Generic;
using Domivium.Client.Contents.UI.Stack;
using Domivium.Client.Data.Item;
using UnityEngine;

namespace Domivium.Client.Contents.UIComponents.ItemContainer
{
    public abstract class ItemContainer : MonoBehaviour
    {
        [SerializeField] protected List<ItemSlot> _slots;

        private Camera _uiCamera;

        public abstract void Reset();

        public void Initialize(Camera uiCamera, IItemContainerStackUIMessage message)
        {
            _uiCamera = uiCamera;

            foreach (var slot in _slots)
            {
                slot.SelectItem = message.OnClick;
                slot.PickItem = message.OnBeginDrag;
                slot.MoveItem = message.OnDrag;
                slot.DropItem = message.OnEndDrag;
            }
        }

        public ItemSlot GetSlot(int index) => _slots[index];

        public bool TryGetSlot(Vector2 position, out ItemSlotData selectedSlot)
        {
            selectedSlot = default;

            foreach (var slot in _slots)
            {
                if (!slot.isActiveAndEnabled) continue;

                if (!RectTransformUtility.RectangleContainsScreenPoint(slot.RectTransform, position, _uiCamera)) continue;

                selectedSlot = slot.SlotInfo;
                return true;
            }

            return false;
        }

        public virtual void SetSlot(int slotCount, int index, Sprite sprite, int itemCount, bool isStackable)
        {
            _slots[index].Set(sprite, itemCount, isStackable);
        }

        public virtual void ClearSlot(int slotCount, int index)
        {
            _slots[index].Clear();
        }
    }
}