using System;
using Domivium.Client.Data.Item;

namespace Domivium.Client.Core.Utility
{
    public static class Converter
    {
        public static ItemType GetItemType(int slotIndex)
        {
            return slotIndex switch
            {
                0 => ItemType.Weapon,
                1 => ItemType.Helmet,
                2 => ItemType.Necklace,
                3 => ItemType.Backpack,
                4 => ItemType.Projectile,
                5 => ItemType.Armor,
                6 => ItemType.Ring,
                _ => throw new ArgumentOutOfRangeException(nameof(slotIndex))
            };
        }

        public static int GetSlotIndex(ItemType itemType)
        {
            return itemType switch
            {
                ItemType.Weapon => 0,
                ItemType.Helmet => 1,
                ItemType.Necklace => 2,
                ItemType.Backpack => 3,
                ItemType.Projectile => 4,
                ItemType.Armor => 5,
                ItemType.Ring => 6,
                _ => throw new ArgumentOutOfRangeException(nameof(itemType))
            };
        }
    }
}