using Cysharp.Threading.Tasks;
using Domivium.Client.Data.Item;

namespace Domivium.Client.Core.Systems
{
    public interface IItemSystemCommand
    {
        public UniTask RunAsync();
        public void Equip(ItemSlotEntry fromSlot, ItemSlotEntry toSlot);
        public void Unequip(ItemSlotEntry fromSlot, ItemSlotEntry toSlot);
        public void SetInventoryCapacity(int capacity);
        public void SetLootCapacity(int capacity);
        public void SwapOrMerge(ItemSlotEntry fromSlot, ItemSlotEntry toSlot);
        public void SplitStack(ItemSlotEntry sourceSlot, int amount);
        public void Remove(ItemSlotEntry slot);
    }
}