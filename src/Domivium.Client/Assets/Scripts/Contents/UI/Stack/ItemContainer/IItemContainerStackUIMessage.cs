using Domivium.Client.Core.UI;
using Domivium.Client.Data.Item;
using UnityEngine;

namespace Domivium.Client.Contents.UI.Stack
{
    public interface IItemContainerStackUIMessage : IUIMessage
    {
        public void OnClick(ItemSlotEntry slot);
        public void OnBeginDrag(ItemSlotEntry slot, Vector2 position);
        public void OnDrag(Vector2 position);
        public void OnEndDrag(ItemSlotEntry sourceSlot, Vector2 position);
        public void OnEquip(ItemSlotEntry slot);
        public void OnUnequip(ItemSlotEntry slot);
        public void OnShowSplitter(ItemSlotEntry slot);
        public void OnUse(ItemSlotEntry slot);
        public void OnKeep(ItemSlotEntry slot);
        public void OnTakeOut(ItemSlotEntry slot);
        public void OnRemove(ItemSlotEntry slot);
        public void OnSplit(ItemSlotEntry slot, int count);
    }
}