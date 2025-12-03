using System;
using Domivium.Client.Data.Context;
using Domivium.Client.Data.Item;
using Domivium.Client.Data.Loot;
using UnityEngine;

namespace Domivium.Client.Contents.Services
{
    public sealed class MasterDbService
    {
        public MemoryDatabase DB { get; }

        public MasterDbService(TextAsset data)
        {
            this.Log();
            DB = new MemoryDatabase(data.bytes);
        }

        public ItemContext GetItemContext(ItemType itemType, int itemId)
        {
            switch (itemType)
            {
                case ItemType.Weapon:
                    return new ItemContext(DB.WeaponRowTable.FindById(itemId));
                case ItemType.Projectile:
                    return new ItemContext(DB.ProjectileRowTable.FindById(itemId));
                case ItemType.Ring:
                case ItemType.Necklace:
                case ItemType.Head:
                case ItemType.Body:
                case ItemType.Feet:
                case ItemType.Bag:
                case ItemType.Potion:
                case ItemType.None:
                case ItemType.Food:
                case ItemType.Cash:
                case ItemType.Material:
                default:
                    throw new ArgumentOutOfRangeException(nameof(itemType), itemType, null);
            }
        }

        public LootContext GetLootContext(LootType lootType, int id)
        {
            switch (lootType)
            {
                case LootType.WeaponBox:
                    return new LootContext(lootType, id, DB.WeaponBoxRowTable.FindByGroupId(id));
                case LootType.ArmorBox:
                    return new LootContext(lootType, id, DB.ArmorBoxRowTable.FindByGroupId(id));
                case LootType.FoodBox:
                    return new LootContext(lootType, id, DB.FoodBoxRowTable.FindByGroupId(id));
                case LootType.PotionBox:
                    return new LootContext(lootType, id, DB.PotionBoxRowTable.FindByGroupId(id));
                case LootType.MaterialBox:
                    return new LootContext(lootType, id, DB.MaterialBoxRowTable.FindByGroupId(id));
                case LootType.CashBox:
                    return new LootContext(lootType, id, DB.CashBoxRowTable.FindByGroupId(id));
                case LootType.EventBox:
                //todo: return new LootContext(lootType, id, DB.EventBoxRowTable.FindByGroupId(id));
                case LootType.None:
                case LootType.CharacterTombStone:
                case LootType.MonsterTombStone:
                default:
                    throw new ArgumentOutOfRangeException(nameof(lootType), lootType, null);
            }
        }
    }
}