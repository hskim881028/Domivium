using System;
using Domivium.Client.Data.DataTransferObject;

namespace Domivium.Client.Data.Item
{
    public sealed class ItemEntity
    {
        public Guid Guid { get; }
        public ItemType Type { get; }
        public int Id { get; }
        public int Count { get; }
        public bool IsStackable { get; }

        public ItemEntity(
            Guid guid,
            ItemType type,
            int id,
            int count,
            bool isStackable)
        {
            Guid = guid;
            Id = id;
            Type = type;
            Count = count;
            IsStackable = isStackable;
        }

        public ItemEntity AddCount(int value) => new(Guid.NewGuid(), Type, Id, Count + value, IsStackable);

        public ItemEntity RemoveCount(int value) => new(Guid.NewGuid(), Type, Id, Count - value, IsStackable);

        public (ItemEntity oldItem, ItemEntity newItem) Split(int value)
        {
            var oldItem = new ItemEntity(Guid.NewGuid(), Type, Id, Count - value, IsStackable);
            var newItem = new ItemEntity(Guid.NewGuid(), Type, Id, value, IsStackable);
            return (oldItem, newItem);
        }

        public bool CanMerge(ItemEntity other) => IsStackable && other.IsStackable && Id == other.Id && Type == other.Type;

        public ItemDto ToDto(int slotIndex) => new()
        {
            SlotIndex = slotIndex,
            Guid = Guid,
            ItemType = Type,
            ItemId = Id,
            ItemCount = Count,
            IsStackable = IsStackable
        };
    }
}