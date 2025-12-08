using System;
using System.Collections.Generic;
using Domivium.Client.Data.Item;
using Domivium.Client.Data.Loot;
using Domivium.Client.Data.Row;
using MasterMemory;

namespace Domivium.Client.Data.Context
{
    public class LootsTable
    {
        public record LootTable(ItemType ItemType, int ItemId, int MinCount, int MaxCount, int DropRate);

        public LootType LootType { get; }
        public int Id { get; }
        public string ItemType { get; set; } = string.Empty;
        public IReadOnlyList<LootTable> Loots { get; }

        public LootsTable(LootType lootType, int id, RangeView<MonsterBoxRow> rows)
        {
            LootType = lootType;
            Id = id;
            var loots = new List<LootTable>();
            foreach (var row in rows)
            {
                var itemType = Enum.Parse<ItemType>(row.ItemType);
                loots.Add(new LootTable(itemType, row.ItemId, row.MinCount, row.MaxCount, row.DropRate));
            }

            Loots = loots;
        }

        public LootsTable(LootType lootType, int id, RangeView<WeaponBoxRow> rows)
        {
            LootType = lootType;
            Id = id;
            var loots = new List<LootTable>();
            foreach (var row in rows)
            {
                var itemType = Enum.Parse<ItemType>(row.ItemType);
                loots.Add(new LootTable(itemType, row.ItemId, row.MinCount, row.MaxCount, row.DropRate));
            }

            Loots = loots;
        }

        public LootsTable(LootType lootType, int id, RangeView<ArmorBoxRow> rows)
        {
            LootType = lootType;
            Id = id;
            var loots = new List<LootTable>();
            foreach (var row in rows)
            {
                var itemType = Enum.Parse<ItemType>(row.ItemType);
                loots.Add(new LootTable(itemType, row.Id, row.MinCount, row.MaxCount, row.DropRate));
            }

            Loots = loots;
        }

        public LootsTable(LootType lootType, int id, RangeView<FoodBoxRow> rows)
        {
            LootType = lootType;
            Id = id;
            var loots = new List<LootTable>();
            foreach (var row in rows)
            {
                var itemType = Enum.Parse<ItemType>(row.ItemType);
                loots.Add(new LootTable(itemType, row.Id, row.MinCount, row.MaxCount, row.DropRate));
            }

            Loots = loots;
        }

        public LootsTable(LootType lootType, int id, RangeView<PotionBoxRow> rows)
        {
            LootType = lootType;
            Id = id;
            var loots = new List<LootTable>();
            foreach (var row in rows)
            {
                var itemType = Enum.Parse<ItemType>(row.ItemType);
                loots.Add(new LootTable(itemType, row.Id, row.MinCount, row.MaxCount, row.DropRate));
            }

            Loots = loots;
        }

        public LootsTable(LootType lootType, int id, RangeView<MaterialBoxRow> rows)
        {
            LootType = lootType;
            Id = id;
            var loots = new List<LootTable>();
            foreach (var row in rows)
            {
                var itemType = Enum.Parse<ItemType>(row.ItemType);
                loots.Add(new LootTable(itemType, row.Id, row.MinCount, row.MaxCount, row.DropRate));
            }

            Loots = loots;
        }

        public LootsTable(LootType lootType, int id, RangeView<CashBoxRow> rows)
        {
            LootType = lootType;
            Id = id;
            var loots = new List<LootTable>();
            foreach (var row in rows)
            {
                var itemType = Enum.Parse<ItemType>(row.ItemType);
                loots.Add(new LootTable(itemType, row.Id, row.MinCount, row.MaxCount, row.DropRate));
            }

            Loots = loots;
        }
    }
}