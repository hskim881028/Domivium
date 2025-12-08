using System.Collections.Generic;
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
        public bool TryToDto(out LootsDto data);
        public bool TryGetTombstones(out IReadOnlyList<LootEntity> loots);
        public bool TryGetItem(int slotIndex, out ItemEntity item);
        public bool TryGetEmptySlotIndex(out int slotIndex);
        public bool IsValidSlot(int slotIndex);
        public bool IsExistLoot(Vector2 position);
        public void AddLoot(LootEntity loot);
        public void SetItem(int slotIndex, ItemEntity item);
        public void MergeItem(int slotIndex, int count);
        public void RemoveItem(int slotIndex);
    }
}