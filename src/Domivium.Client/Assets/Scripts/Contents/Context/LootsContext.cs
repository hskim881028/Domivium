using Domivium.Client.Core.Context;
using Domivium.Client.Data.DataTransferObject;
using Domivium.Client.Data.Item;
using Domivium.Client.Data.Loot;
using R3;
using UnityEngine;

namespace Domivium.Client.Contents.Context
{
    public sealed class LootsContext : ILootsContext
    {
        private readonly Loots _loots = new();
        private readonly ReactiveProperty<LootEntity> _loot = new();

        public ReadOnlyReactiveProperty<LootEntity> Loot => _loot;

        public bool FoundLoot => _loot.CurrentValue != null;

        public void Initialize(Transform character, LootsDto data)
        {
            _loots.SetData(character, data);
        }

        public bool IsValid(int slotIndex)
        {
            if (_loot.CurrentValue == null) return false;

            return slotIndex >= 0 && slotIndex < _loot.CurrentValue.Capacity;
        }

        public bool TryToDto(out LootsDto data) => _loots.ToDto(out data);

        public bool TryGetItem(int slotIndex, out ItemEntity item)
        {
            item = null;
            return _loot.CurrentValue != null && _loot.CurrentValue.Items.TryGetValue(slotIndex, out item);
        }

        public bool TryGetEmptySlotIndex(out int slotIndex)
        {
            slotIndex = -1;
            if (_loot.CurrentValue == null) return false;

            for (var i = 0; i < _loot.CurrentValue.Capacity; i++)
            {
                if (_loot.CurrentValue.Items.ContainsKey(i)) continue;

                slotIndex = i;
                return true;
            }

            return false;
        }

        public void Remove(int slotIndex)
        {
            if (_loot.CurrentValue == null) return;

            _loot.CurrentValue.Items.Remove(slotIndex);
            _loot.ForceNotify();
        }

        public void Set(int slotIndex, ItemEntity item)
        {
            if (_loot.CurrentValue == null) return;

            _loot.CurrentValue.Items[slotIndex] = item;
            _loot.ForceNotify();
        }

        public void Merge(int slotIndex, int count)
        {
            if (_loot.CurrentValue == null) return;

            _loot.CurrentValue.Items[slotIndex] = _loot.CurrentValue.Items[slotIndex].AddCount(count);
            _loot.ForceNotify();
        }

        public void Tick(float deltaTime)
        {
            _loot.Value = _loots.FindNearestLoot();
        }
    }
}