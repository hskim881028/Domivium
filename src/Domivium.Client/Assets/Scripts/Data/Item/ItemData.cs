using System;

namespace Domivium.Client.Data.Item
{
    public struct ItemData
    {
        public ItemType Type { get; }

        public int Id { get; }

        public int Count { get; private set; }

        public bool IsStackable => Type is ItemType.Projectile or ItemType.Food or ItemType.Potion;

        public ItemData(ItemType type, int id, int count)
        {
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

        public ItemData Split(int amount)
        {
            if (!IsStackable)
            {
                throw new InvalidOperationException("Non stackable item cannot be split.");
            }

            if (amount <= 0 || amount >= Count)
            {
                throw new ArgumentOutOfRangeException(nameof(amount));
            }

            Count -= amount;
            return new ItemData(Type, Id, amount);
        }

        public bool CanMerge(ItemData other)
        {
            return IsStackable &&
                   other.IsStackable &&
                   Id == other.Id &&
                   Type == other.Type;
        }
    }
}