using System;
using Domivium.Client.Core.Actors;
using Domivium.Client.Data.Rarity;
using Domivium.Client.Data.Row;

namespace Domivium.Client.Contents.Actors
{
    public class ProjectileContext
    {
        public ActorId ActorId { get; }
        public int Id { get; }
        public string Type { get; }
        public RarityType RarityType { get; }
        public int Durability { get; }
        public int Attack { get; }
        public int AttackRange { get; }
        public int ProjectileSpeed { get; }
        public int CriticalRate { get; }
        public int CriticalDamage { get; }

        public ProjectileContext(ProjectileRow row)
        {
            ActorId = ActorId.Projectile;
            Id = row.Id;
            Type = row.Type;
            RarityType = Enum.Parse<RarityType>(row.Rarity);
            Durability = row.Durability;
            Attack = row.Attack;
            AttackRange = row.AttackRange;
            ProjectileSpeed = row.ProjectileSpeed;
            CriticalRate = row.CriticalRate;
            CriticalDamage = row.CriticalDamage;
        }
    }
}