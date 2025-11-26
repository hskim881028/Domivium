using Cysharp.Threading.Tasks;
using Domivium.Client.Data.Item;

namespace Domivium.Client.Core.Systems
{
    public interface IItemSystemCommand
    {
        public UniTask RunAsync();
        public void Equip(ItemSlotData fromSlot, ItemSlotData toSlot);
        public void Unequip(ItemSlotData fromSlot, ItemSlotData toSlot);
        public void SetInventoryCapacity(int capacity);
        public void SetLootCapacity(int capacity);
        public void SwapOrMerge(ItemSlotData fromSlot, ItemSlotData toSlot);
        public void SplitStack(ItemSlotData sourceSlot, int amount);
        public void Remove(ItemSlotData slot);
    }
}