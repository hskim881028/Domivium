using System;
using System.Collections.Generic;
using Domivium.Client.Data.Item;
using UnityEngine;

namespace Domivium.Client.Data.Loot
{
    public class LootEntity
    {
        public Guid Guid { get; }
        public Vector2 Position { get; }
        public LootType Type { get; }
        public Dictionary<int, ItemEntity> Items { get; }
        public int Capacity { get; }
        public bool IsOpened { get; private set; }

        public LootEntity(
            Guid guid,
            Vector2 position,
            LootType type,
            Dictionary<int, ItemEntity> items,
            int capacity,
            bool isOpened = false)
        {
            Guid = guid;
            Position = position;
            Type = type;
            Items = items;
            Capacity = capacity;
            IsOpened = isOpened;
        }

        public void Open()
        {
            IsOpened = true;
        }
    }
}