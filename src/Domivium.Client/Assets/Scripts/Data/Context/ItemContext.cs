using System;
using Domivium.Client.Data.Rarity;
using Domivium.Client.Data.Row;

namespace Domivium.Client.Data.Context
{
    public class ItemContext
    {
        public int Id { get; }
        public string Type { get; }
        public RarityType RarityType { get; }
        public int Durability { get; }
        public int Weight { get; }
        public int MaxWeight { get; }
        public int InventoryCapacity { get; }
        public int ProjectileCapacity { get; }
        public int Attack { get; }
        public int Defense { get; }
        public int AttackRange { get; }
        public int HitRange { get; }
        public int MoveSpeed { get; }
        public int AttackSpeed { get; }
        public int ReloadSpeed { get; }
        public int CriticalRate { get; }
        public int CriticalDamage { get; }

        public ItemContext(WeaponRow row)
        {
            Id = row.Id;
            Type = row.Type;
            RarityType = Enum.Parse<RarityType>(row.Rarity);
            Durability = row.Durability;
            Weight = row.Weight;
            MaxWeight = row.MaxWeight;
            InventoryCapacity = row.InventoryCapacity;
            ProjectileCapacity = row.ProjectileCapacity;
            Attack = row.Attack;
            Defense = row.Defense;
            AttackRange = row.AttackRange;
            HitRange = row.HitRange;
            MoveSpeed = row.MoveSpeed;
            AttackSpeed = row.AttackSpeed;
            ReloadSpeed = row.ReloadSpeed;
            CriticalRate = row.CriticalRate;
            CriticalDamage = row.CriticalDamage;
        }

        public ItemContext(ProjectileRow row)
        {
            Id = row.Id;
            Type = row.Type;
            RarityType = Enum.Parse<RarityType>(row.Rarity);
            Durability = row.Durability;
            Weight = row.Weight;
            MaxWeight = row.MaxWeight;
            InventoryCapacity = row.InventoryCapacity;
            ProjectileCapacity = row.ProjectileCapacity;
            Attack = row.Attack;
            Defense = row.Defense;
            AttackRange = row.AttackRange;
            HitRange = row.HitRange;
            MoveSpeed = row.MoveSpeed;
            AttackSpeed = row.AttackSpeed;
            ReloadSpeed = row.ReloadSpeed;
            CriticalRate = row.CriticalRate;
            CriticalDamage = row.CriticalDamage;
        }
    }
}