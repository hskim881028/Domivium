using System;

namespace Domivium.Client.Data.Item
{
    public struct ItemEntity
    {
        public Guid Guid { get; }
        public ItemType Type { get; }
        public int Id { get; }
        public int Count { get; private set; }

        public bool IsStackable => Type is ItemType.Projectile or ItemType.Food or ItemType.Potion;

        public ItemEntity(Guid guid, ItemType type, int id, int count)
        {
            Guid = guid;
            Id = id;
            Type = type;
            Count = count;
        }

        public void Add(int value)
        {
            if (!IsStackable)
            {
                throw new InvalidOperationException("Non stackable item cannot change count.");
            }

            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            Count += value;
        }

        public ItemEntity Remove(int value)
        {
            if (!IsStackable)
            {
                throw new InvalidOperationException("Non stackable item cannot be split.");
            }

            if (value <= 0 || value > Count)
            {
                throw new ArgumentOutOfRangeException($"value: {value}({Count})");
            }

            Count -= value;
            return new ItemEntity(Guid, Type, Id, Count);
        }

        public ItemEntity Split(int value)
        {
            if (!IsStackable)
            {
                throw new InvalidOperationException("Non stackable item cannot be split.");
            }

            if (value <= 0 || value >= Count)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            Count -= value;
            return new ItemEntity(Guid.NewGuid(), Type, Id, value);
        }

        public bool CanMerge(ItemEntity other) => IsStackable && other.IsStackable && Id == other.Id && Type == other.Type;
    }
}