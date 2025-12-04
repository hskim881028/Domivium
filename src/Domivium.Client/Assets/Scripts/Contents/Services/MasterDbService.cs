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

        public ItemTable GetItemTable(ItemType itemType, int itemId)
        {
            switch (itemType)
            {
                case ItemType.Weapon:
                    return new ItemTable(DB.WeaponRowTable.FindById(itemId));
                case ItemType.Projectile:
                    return new ItemTable(DB.ProjectileRowTable.FindById(itemId));
                case ItemType.Ring:
                    return new ItemTable(DB.RingRowTable.FindById(itemId));
                case ItemType.Necklace:
                    return new ItemTable(DB.NecklaceRowTable.FindById(itemId));
                case ItemType.Head:
                    return new ItemTable(DB.HeadRowTable.FindById(itemId));
                case ItemType.Body:
                    return new ItemTable(DB.BagRowTable.FindById(itemId));
                case ItemType.Feet:
                    return new ItemTable(DB.FeetRowTable.FindById(itemId));
                case ItemType.Bag:
                    return new ItemTable(DB.BagRowTable.FindById(itemId));
                case ItemType.Potion:
                    return new ItemTable(DB.PotionRowTable.FindById(itemId));
                case ItemType.Food:
                    return new ItemTable(DB.FeetRowTable.FindById(itemId));
                case ItemType.Cash:
                    return new ItemTable(DB.CashRowTable.FindById(itemId));
                case ItemType.Material:
                    return new ItemTable(DB.MaterialRowTable.FindById(itemId));
                case ItemType.None:
                default:
                    throw new ArgumentOutOfRangeException(nameof(itemType), itemType, null);
            }
        }

        public LootsTable GetLootsTable(LootType lootType, int id)
        {
            switch (lootType)
            {
                case LootType.MonsterBox:
                    return new LootsTable(lootType, id, DB.MonsterBoxRowTable.FindByGroupId(id));
                case LootType.WeaponBox:
                    return new LootsTable(lootType, id, DB.WeaponBoxRowTable.FindByGroupId(id));
                case LootType.ArmorBox:
                    return new LootsTable(lootType, id, DB.ArmorBoxRowTable.FindByGroupId(id));
                case LootType.FoodBox:
                    return new LootsTable(lootType, id, DB.FoodBoxRowTable.FindByGroupId(id));
                case LootType.PotionBox:
                    return new LootsTable(lootType, id, DB.PotionBoxRowTable.FindByGroupId(id));
                case LootType.MaterialBox:
                    return new LootsTable(lootType, id, DB.MaterialBoxRowTable.FindByGroupId(id));
                case LootType.CashBox:
                    return new LootsTable(lootType, id, DB.CashBoxRowTable.FindByGroupId(id));
                case LootType.EventBox:
                //todo: return new LootContext(lootType, id, DB.EventBoxRowTable.FindByGroupId(id));
                case LootType.None:
                case LootType.CharacterBox:
                default:
                    throw new ArgumentOutOfRangeException(nameof(lootType), lootType, null);
            }
        }
    }
}