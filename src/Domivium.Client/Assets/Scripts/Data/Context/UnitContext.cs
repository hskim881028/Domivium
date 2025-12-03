using System;
using Domivium.Client.Core.Actors;
using Domivium.Client.Data.Rarity;
using Domivium.Client.Data.Row;

namespace Domivium.Client.Data.Context
{
    public readonly struct UnitContext
    {
        public ActorId ActorId { get; }
        public RarityType RarityType { get; }
        public int Id { get; }
        public string Type { get; }
        public int Health { get; }
        public int Hunger { get; }
        public int Stamina { get; }
        public int Sanity { get; }
        public int Durability { get; }
        public int Weight { get; }
        public int WeightCapacity { get; }
        public int InventoryCapacity { get; }
        public int ProjectileCapacity { get; }
        public int Attack { get; }
        public int Defense { get; }
        public int Penetration { get; }
        public int AttackRange { get; }
        public int DetectionRange { get; }
        public int MoveSpeed { get; }
        public int AttackSpeed { get; }
        public int ReloadSpeed { get; }
        public int ProjectileSpeed { get; }
        public int CriticalRate { get; }
        public int CriticalDamage { get; }


        public UnitContext(CharacterRow row)
        {
            ActorId = ActorId.Character;
            Id = row.Id;
            RarityType = Enum.Parse<RarityType>(row.Rarity);
            Type = row.Type;
            Health = row.Health;
            Hunger = row.Hunger;
            Stamina = row.Stamina;
            Sanity = row.Sanity;
            Durability = row.Durability;
            Weight = row.Weight;
            WeightCapacity = row.WeightCapacity;
            InventoryCapacity = row.InventoryCapacity;
            ProjectileCapacity = row.ProjectileCapacity;
            Attack = row.Attack;
            Defense = row.Defense;
            Penetration = row.Penetration;
            AttackRange = row.AttackRange;
            DetectionRange = row.DetectionRange;
            MoveSpeed = row.MoveSpeed;
            AttackSpeed = row.AttackSpeed;
            ReloadSpeed = row.ReloadSpeed;
            ProjectileSpeed = row.ProjectileSpeed;
            CriticalRate = row.CriticalRate;
            CriticalDamage = row.CriticalDamage;
        }

        public UnitContext(MonsterRow row)
        {
            ActorId = ActorId.Monster;
            RarityType = Enum.Parse<RarityType>(row.Rarity);

            Id = row.Id;
            Type = row.Type;
            Health = row.Health;
            Hunger = row.Hunger;
            Stamina = row.Stamina;
            Sanity = row.Sanity;
            Durability = row.Durability;
            Weight = row.Weight;
            WeightCapacity = row.WeightCapacity;
            InventoryCapacity = row.InventoryCapacity;
            ProjectileCapacity = row.ProjectileCapacity;
            Attack = row.Attack;
            Defense = row.Defense;
            Penetration = row.Penetration;
            AttackRange = row.AttackRange;
            DetectionRange = row.DetectionRange;
            MoveSpeed = row.MoveSpeed;
            AttackSpeed = row.AttackSpeed;
            ReloadSpeed = row.ReloadSpeed;
            ProjectileSpeed = row.ProjectileSpeed;
            CriticalRate = row.CriticalRate;
            CriticalDamage = row.CriticalDamage;
        }
    }
}