using System;
using Domivium.Client.Data.Rarity;
using Domivium.Client.Data.Row;

namespace Domivium.Client.Data.Context
{
    public class ItemTable
    {
        public int Id { get; }
        public string Type { get; }
        public RarityType RarityType { get; }
        public int Durability { get; }
        public int Weight { get; }
        public int WeightCapacity { get; }
        public int InventoryCapacity { get; }
        public int ProjectileCapacity { get; }
        public int Attack { get; }
        public int Defense { get; }
        public int Penetration { get; }
        public int AttackRange { get; }
        public int MoveSpeed { get; }
        public int ProjectileSpeed { get; }
        public int AttackSpeed { get; }
        public int ReloadSpeed { get; }
        public int CriticalRate { get; }
        public int CriticalDamage { get; }

        public ItemTable(WeaponRow row)
        {
            Id = row.Id;
            Type = row.Type;
            RarityType = Enum.Parse<RarityType>(row.Rarity);
            Durability = row.Durability;
            Weight = row.Weight;
            WeightCapacity = row.WeightCapacity;
            InventoryCapacity = row.InventoryCapacity;
            ProjectileCapacity = row.ProjectileCapacity;
            Attack = row.Attack;
            Defense = row.Defense;
            Penetration = row.Penetration;
            AttackRange = row.AttackRange;
            MoveSpeed = row.MoveSpeed;
            AttackSpeed = row.AttackSpeed;
            ReloadSpeed = row.ReloadSpeed;
            ProjectileSpeed = row.ProjectileSpeed;
            CriticalRate = row.CriticalRate;
            CriticalDamage = row.CriticalDamage;
        }

        public ItemTable(ProjectileRow row)
        {
            Id = row.Id;
            Type = row.Type;
            RarityType = Enum.Parse<RarityType>(row.Rarity);
            Durability = row.Durability;
            Weight = row.Weight;
            WeightCapacity = row.WeightCapacity;
            InventoryCapacity = row.InventoryCapacity;
            ProjectileCapacity = row.ProjectileCapacity;
            Attack = row.Attack;
            Defense = row.Defense;
            Penetration = row.Penetration;
            AttackRange = row.AttackRange;
            MoveSpeed = row.MoveSpeed;
            AttackSpeed = row.AttackSpeed;
            ReloadSpeed = row.ReloadSpeed;
            ProjectileSpeed = row.ProjectileSpeed;
            CriticalRate = row.CriticalRate;
            CriticalDamage = row.CriticalDamage;
        }

        public ItemTable(RingRow row)
        {
            Id = row.Id;
            Type = row.Type;
            RarityType = Enum.Parse<RarityType>(row.Rarity);
            Durability = row.Durability;
            Weight = row.Weight;
            WeightCapacity = row.WeightCapacity;
            InventoryCapacity = row.InventoryCapacity;
            ProjectileCapacity = row.ProjectileCapacity;
            Attack = row.Attack;
            Defense = row.Defense;
            Penetration = row.Penetration;
            AttackRange = row.AttackRange;
            MoveSpeed = row.MoveSpeed;
            AttackSpeed = row.AttackSpeed;
            ReloadSpeed = row.ReloadSpeed;
            ProjectileSpeed = row.ProjectileSpeed;
            CriticalRate = row.CriticalRate;
            CriticalDamage = row.CriticalDamage;
        }

        public ItemTable(NecklaceRow row)
        {
            Id = row.Id;
            Type = row.Type;
            RarityType = Enum.Parse<RarityType>(row.Rarity);
            Durability = row.Durability;
            Weight = row.Weight;
            WeightCapacity = row.WeightCapacity;
            InventoryCapacity = row.InventoryCapacity;
            ProjectileCapacity = row.ProjectileCapacity;
            Attack = row.Attack;
            Defense = row.Defense;
            Penetration = row.Penetration;
            AttackRange = row.AttackRange;
            MoveSpeed = row.MoveSpeed;
            AttackSpeed = row.AttackSpeed;
            ReloadSpeed = row.ReloadSpeed;
            ProjectileSpeed = row.ProjectileSpeed;
            CriticalRate = row.CriticalRate;
            CriticalDamage = row.CriticalDamage;
        }

        public ItemTable(HeadRow row)
        {
            Id = row.Id;
            Type = row.Type;
            RarityType = Enum.Parse<RarityType>(row.Rarity);
            Durability = row.Durability;
            Weight = row.Weight;
            WeightCapacity = row.WeightCapacity;
            InventoryCapacity = row.InventoryCapacity;
            ProjectileCapacity = row.ProjectileCapacity;
            Attack = row.Attack;
            Defense = row.Defense;
            Penetration = row.Penetration;
            AttackRange = row.AttackRange;
            MoveSpeed = row.MoveSpeed;
            AttackSpeed = row.AttackSpeed;
            ReloadSpeed = row.ReloadSpeed;
            ProjectileSpeed = row.ProjectileSpeed;
            CriticalRate = row.CriticalRate;
            CriticalDamage = row.CriticalDamage;
        }

        public ItemTable(BodyRow row)
        {
            Id = row.Id;
            Type = row.Type;
            RarityType = Enum.Parse<RarityType>(row.Rarity);
            Durability = row.Durability;
            Weight = row.Weight;
            WeightCapacity = row.WeightCapacity;
            InventoryCapacity = row.InventoryCapacity;
            ProjectileCapacity = row.ProjectileCapacity;
            Attack = row.Attack;
            Defense = row.Defense;
            Penetration = row.Penetration;
            AttackRange = row.AttackRange;
            MoveSpeed = row.MoveSpeed;
            AttackSpeed = row.AttackSpeed;
            ReloadSpeed = row.ReloadSpeed;
            ProjectileSpeed = row.ProjectileSpeed;
            CriticalRate = row.CriticalRate;
            CriticalDamage = row.CriticalDamage;
        }

        public ItemTable(FeetRow row)
        {
            Id = row.Id;
            Type = row.Type;
            RarityType = Enum.Parse<RarityType>(row.Rarity);
            Durability = row.Durability;
            Weight = row.Weight;
            WeightCapacity = row.WeightCapacity;
            InventoryCapacity = row.InventoryCapacity;
            ProjectileCapacity = row.ProjectileCapacity;
            Attack = row.Attack;
            Defense = row.Defense;
            Penetration = row.Penetration;
            AttackRange = row.AttackRange;
            MoveSpeed = row.MoveSpeed;
            AttackSpeed = row.AttackSpeed;
            ReloadSpeed = row.ReloadSpeed;
            ProjectileSpeed = row.ProjectileSpeed;
            CriticalRate = row.CriticalRate;
            CriticalDamage = row.CriticalDamage;
        }

        public ItemTable(BagRow row)
        {
            Id = row.Id;
            Type = row.Type;
            RarityType = Enum.Parse<RarityType>(row.Rarity);
            Durability = row.Durability;
            Weight = row.Weight;
            WeightCapacity = row.WeightCapacity;
            InventoryCapacity = row.InventoryCapacity;
            ProjectileCapacity = row.ProjectileCapacity;
            Attack = row.Attack;
            Defense = row.Defense;
            Penetration = row.Penetration;
            AttackRange = row.AttackRange;
            MoveSpeed = row.MoveSpeed;
            AttackSpeed = row.AttackSpeed;
            ReloadSpeed = row.ReloadSpeed;
            ProjectileSpeed = row.ProjectileSpeed;
            CriticalRate = row.CriticalRate;
            CriticalDamage = row.CriticalDamage;
        }

        public ItemTable(PotionRow row)
        {
            Id = row.Id;
            Type = row.Type;
            RarityType = Enum.Parse<RarityType>(row.Rarity);
            Durability = row.Durability;
            Weight = row.Weight;
            WeightCapacity = row.WeightCapacity;
            InventoryCapacity = row.InventoryCapacity;
            ProjectileCapacity = row.ProjectileCapacity;
            Attack = row.Attack;
            Defense = row.Defense;
            Penetration = row.Penetration;
            AttackRange = row.AttackRange;
            MoveSpeed = row.MoveSpeed;
            AttackSpeed = row.AttackSpeed;
            ReloadSpeed = row.ReloadSpeed;
            ProjectileSpeed = row.ProjectileSpeed;
            CriticalRate = row.CriticalRate;
            CriticalDamage = row.CriticalDamage;
        }

        public ItemTable(FoodRow row)
        {
            Id = row.Id;
            Type = row.Type;
            RarityType = Enum.Parse<RarityType>(row.Rarity);
            Durability = row.Durability;
            Weight = row.Weight;
            WeightCapacity = row.WeightCapacity;
            InventoryCapacity = row.InventoryCapacity;
            ProjectileCapacity = row.ProjectileCapacity;
            Attack = row.Attack;
            Defense = row.Defense;
            Penetration = row.Penetration;
            AttackRange = row.AttackRange;
            MoveSpeed = row.MoveSpeed;
            AttackSpeed = row.AttackSpeed;
            ReloadSpeed = row.ReloadSpeed;
            ProjectileSpeed = row.ProjectileSpeed;
            CriticalRate = row.CriticalRate;
            CriticalDamage = row.CriticalDamage;
        }

        public ItemTable(CashRow row)
        {
            Id = row.Id;
            Type = row.Type;
            RarityType = Enum.Parse<RarityType>(row.Rarity);
            Durability = row.Durability;
            Weight = row.Weight;
            WeightCapacity = row.WeightCapacity;
            InventoryCapacity = row.InventoryCapacity;
            ProjectileCapacity = row.ProjectileCapacity;
            Attack = row.Attack;
            Defense = row.Defense;
            Penetration = row.Penetration;
            AttackRange = row.AttackRange;
            MoveSpeed = row.MoveSpeed;
            AttackSpeed = row.AttackSpeed;
            ReloadSpeed = row.ReloadSpeed;
            ProjectileSpeed = row.ProjectileSpeed;
            CriticalRate = row.CriticalRate;
            CriticalDamage = row.CriticalDamage;
        }

        public ItemTable(MaterialRow row)
        {
            Id = row.Id;
            Type = row.Type;
            RarityType = Enum.Parse<RarityType>(row.Rarity);
            Durability = row.Durability;
            Weight = row.Weight;
            WeightCapacity = row.WeightCapacity;
            InventoryCapacity = row.InventoryCapacity;
            ProjectileCapacity = row.ProjectileCapacity;
            Attack = row.Attack;
            Defense = row.Defense;
            Penetration = row.Penetration;
            AttackRange = row.AttackRange;
            MoveSpeed = row.MoveSpeed;
            AttackSpeed = row.AttackSpeed;
            ReloadSpeed = row.ReloadSpeed;
            ProjectileSpeed = row.ProjectileSpeed;
            CriticalRate = row.CriticalRate;
            CriticalDamage = row.CriticalDamage;
        }
    }
}