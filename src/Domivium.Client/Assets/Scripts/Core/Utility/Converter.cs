using System;
using Domivium.Client.Data.Item;
using Domivium.Client.Data.Stat;

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

        public static string GetStatName(StatId statId)
        {
            return statId.AsPrimitive() switch
            {
                0 => "생명력",
                1 => "체력",
                2 => "지구력",
                3 => "정신력",
                4 => "내구도",
                5 => "무게",
                6 => "인벤토리 공간",
                7 => "스펠 충전량",
                8 => "공격력",
                9 => "방어력",
                10 => "탐지범위",
                11 => "사거리",
                12 => "이동 속도",
                13 => "공격 속도",
                14 => "스펠 재충전 시간",
                15 => "스펠 속도",
                16 => "치명타율",
                17 => "치명타 피해",
                _ => throw new ArgumentOutOfRangeException(nameof(statId))
            };
        }

        public static string GetItemTypeName(ItemType itemType)
        {
            switch (itemType)
            {
                case ItemType.Weapon:
                    return "무기";
                case ItemType.Helmet:
                    return "투구";
                case ItemType.Necklace:
                    return "목걸이";
                case ItemType.Backpack:
                    return "가방";
                case ItemType.Projectile:
                    return "스펠";
                case ItemType.Armor:
                    return "갑옷";
                case ItemType.Ring:
                    return "반지";
                case ItemType.Food:
                    return "음식";
                case ItemType.Potion:
                    return "물약";
                case ItemType.None:
                default:
                    throw new ArgumentOutOfRangeException(nameof(itemType), itemType, null);
            }
        }

        public static string GetItemName(ItemType itemType, int id) // temp
        {
            switch (itemType)
            {
                case ItemType.Weapon:
                    return id switch
                    {
                        1 => "해리포터 지팡이",
                        _ => throw new ArgumentOutOfRangeException(nameof(id))
                    };
                case ItemType.Helmet:
                    return "투구";
                case ItemType.Necklace:
                    return "목걸이";
                case ItemType.Backpack:
                    return "가방";
                case ItemType.Projectile:
                    return id switch
                    {
                        1 => "파이어볼",
                        2 => "아케인코멧",
                        3 => "트윈플레어",
                        4 => "스타폴샤드",
                        _ => throw new ArgumentOutOfRangeException(nameof(id))
                    };
                case ItemType.Armor:
                    return "갑옷";
                case ItemType.Ring:
                    return "반지";
                case ItemType.Food:
                    return "음식";
                case ItemType.Potion:
                    return "물약";
                case ItemType.None:
                default:
                    throw new ArgumentOutOfRangeException(nameof(itemType), itemType, null);
            }
        }
    }
}