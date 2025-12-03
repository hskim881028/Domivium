using Domivium.Client.Data.DataTransferObject;
using Domivium.Client.Data.Item;
using Domivium.Client.Data.Loot;
using R3;
using UnityEngine;

namespace Domivium.Client.Core.Context
{
    public interface ILootsContext : ITicker
    {
        public ReadOnlyReactiveProperty<LootEntity> Loot { get; }
        public bool FoundLoot { get; }
        public void Initialize(Transform character, LootsDto data);
        public bool IsValid(int slotIndex);
        public bool TryToDto(out LootsDto data);
        public bool TryGetItem(int slotIndex, out ItemEntity item);
        public bool TryGetEmptySlotIndex(out int slotIndex);
        public void Remove(int slotIndex);
        public void Set(int slotIndex, ItemEntity item);
        public void Merge(int slotIndex, int count);
    }
}