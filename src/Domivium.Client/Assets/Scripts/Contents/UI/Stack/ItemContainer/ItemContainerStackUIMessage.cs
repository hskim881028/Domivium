using Domivium.Client.Core.UI;
using Domivium.Client.Data.Item;
using UnityEngine;

namespace Domivium.Client.Contents.UI.Stack
{
    public interface IItemContainerStackUIMessage : IUIMessage
    {
        public void OnClick(ItemSlotData slot);
        public void OnBeginDrag(ItemSlotData slot, Vector2 position);
        public void OnDrag(Vector2 position);
        public void OnEndDrag(ItemSlotData sourceSlot, Vector2 position);
        public void OnEquip(ItemSlotData slot);
        public void OnUnequip(ItemSlotData slot);
        public void OnShowSplitter(ItemSlotData slot);
        public void OnUse(ItemSlotData slot);
        public void OnKeep(ItemSlotData slot);
        public void OnTakeOut(ItemSlotData slot);
        public void OnRemove(ItemSlotData slot);
        public void OnSplit(ItemSlotData slot, int count);
    }
}