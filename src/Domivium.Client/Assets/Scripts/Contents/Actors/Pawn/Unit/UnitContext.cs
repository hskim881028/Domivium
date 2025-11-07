using Domivium.Client.Contents.Actors.Generated;
using Domivium.Client.Core.Actors;
using Domivium.Client.Data.Row;

namespace Domivium.Client.Contents.Actors
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
        public int MaxWeight { get; }
        public int InventoryCapacity { get; }
        public int ProjectileCapacity { get; }
        public int Attack { get; }
        public int Defense { get; }
        public int AttackRange { get; }
        public int HitRange { get; }
        public int DetectionRange { get; }
        public int FieldOfView { get; }
        public int AngleOfView { get; }
        public int MoveSpeed { get; }
        public int AttackSpeed { get; }
        public int ReloadSpeed { get; }
        public int CriticalRate { get; }
        public int CriticalDamage { get; }


        public UnitContext(CharacterRow row)
        {
            ActorId = ActorIds.Character;
            Id = row.Id;
            RarityType = row.Rarity.ToRarityType();
            Type = row.Type;
            Health = row.Health;
            Hunger = row.Hunger;
            Stamina = row.Stamina;
            Sanity = row.Sanity;
            Durability = row.Durability;
            Weight = row.Weight;
            MaxWeight = row.MaxWeight;
            InventoryCapacity = row.InventoryCapacity;
            ProjectileCapacity = row.ProjectileCapacity;
            Attack = row.Attack;
            Defense = row.Defense;
            AttackRange = row.AttackRange;
            HitRange = row.HitRange;
            DetectionRange = row.DetectionRange;
            FieldOfView = row.FieldOfView;
            AngleOfView = row.AngleOfView;
            MoveSpeed = row.MoveSpeed;
            AttackSpeed = row.AttackSpeed;
            ReloadSpeed = row.ReloadSpeed;
            CriticalRate = row.CriticalRate;
            CriticalDamage = row.CriticalDamage;
        }

        public UnitContext(MonsterRow row)
        {
            ActorId = ActorIds.Monster;
            RarityType = row.Rarity.ToRarityType();

            Id = row.Id;
            Type = row.Type;
            Health = row.Health;
            Hunger = row.Hunger;
            Stamina = row.Stamina;
            Sanity = row.Sanity;
            Durability = row.Durability;
            Weight = row.Weight;
            MaxWeight = row.MaxWeight;
            InventoryCapacity = row.InventoryCapacity;
            ProjectileCapacity = row.ProjectileCapacity;
            Attack = row.Attack;
            Defense = row.Defense;
            AttackRange = row.AttackRange;
            HitRange = row.HitRange;
            DetectionRange = row.DetectionRange;
            FieldOfView = row.FieldOfView;
            AngleOfView = row.AngleOfView;
            MoveSpeed = row.MoveSpeed;
            AttackSpeed = row.AttackSpeed;
            ReloadSpeed = row.ReloadSpeed;
            CriticalRate = row.CriticalRate;
            CriticalDamage = row.CriticalDamage;
        }
    }
}