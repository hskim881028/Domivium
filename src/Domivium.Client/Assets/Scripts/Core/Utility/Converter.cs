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
                1 => ItemType.Projectile,
                2 => ItemType.Ring,
                3 => ItemType.Necklace,
                4 => ItemType.Head,
                5 => ItemType.Body,
                6 => ItemType.Feet,
                7 => ItemType.Bag,
                _ => throw new ArgumentOutOfRangeException(nameof(slotIndex))
            };
        }

        public static bool IsStackable(ItemType itemType)
        {
            switch (itemType)
            {
                case ItemType.Weapon:
                case ItemType.Ring:
                case ItemType.Necklace:
                case ItemType.Head:
                case ItemType.Body:
                case ItemType.Feet:
                case ItemType.Bag:
                    return false;
                case ItemType.Projectile:
                case ItemType.Potion:
                case ItemType.Food:
                case ItemType.Cash:
                case ItemType.Material:
                    return true;
                case ItemType.None:
                default:
                    throw new ArgumentOutOfRangeException(nameof(itemType), itemType, null);
            }
        }

        public static bool TryGetEquipmentSlotIndex(ItemType itemType, out int slotIndex)
        {
            slotIndex = -1;
            switch (itemType)
            {
                case ItemType.Weapon:
                    slotIndex = 0;
                    break;
                case ItemType.Projectile:
                    slotIndex = 1;
                    break;
                case ItemType.Ring:
                    slotIndex = 2;
                    break;
                case ItemType.Necklace:
                    slotIndex = 3;
                    break;
                case ItemType.Head:
                    slotIndex = 4;
                    break;
                case ItemType.Body:
                    slotIndex = 5;
                    break;
                case ItemType.Feet:
                    slotIndex = 6;
                    break;
                case ItemType.Bag:
                    slotIndex = 7;
                    break;
                case ItemType.None:
                case ItemType.Potion:
                case ItemType.Food:
                case ItemType.Cash:
                case ItemType.Material:
                default:
                    return false;
            }

            return true;
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
                case ItemType.Projectile:
                    return "스펠";
                case ItemType.Necklace:
                    return "목걸이";
                case ItemType.Ring:
                    return "반지";
                case ItemType.Head:
                    return "모자";
                case ItemType.Body:
                    return "옷";
                case ItemType.Feet:
                    return "신발";
                case ItemType.Bag:
                    return "가방";
                case ItemType.Potion:
                    return "물약";
                case ItemType.Food:
                    return "음식";
                case ItemType.Cash:
                    return "재화";
                case ItemType.Material:
                    return "재료";
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
                        2 => "말포이 지팡이",
                        _ => throw new ArgumentOutOfRangeException(nameof(id))
                    };
                case ItemType.Projectile:
                    return id switch
                    {
                        1 => "파이어볼",
                        2 => "아케인코멧",
                        3 => "트윈플레어",
                        4 => "스타폴샤드",
                        _ => throw new ArgumentOutOfRangeException(nameof(id))
                    };
                case ItemType.Necklace:
                    return "목걸이";
                case ItemType.Ring:
                    return "반지";
                case ItemType.Head:
                    return "모자";
                case ItemType.Body:
                    return "옷";
                case ItemType.Feet:
                    return "신발";
                case ItemType.Bag:
                    return "가방";
                case ItemType.Potion:
                    return "물약";
                case ItemType.Food:
                    return "음식";
                case ItemType.Cash:
                    return "재화";
                case ItemType.Material:
                    return "재료";
                case ItemType.None:
                default:
                    throw new ArgumentOutOfRangeException(nameof(itemType), itemType, null);
            }
        }
    }
}