using Domivium.Client.Contents.Actors.Generated;
using Domivium.Client.Core.Actors;
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
        public int HitRange { get; }
        public int MoveSpeed { get; }
        public int CriticalRate { get; }
        public int CriticalDamage { get; }

        public ProjectileContext(ProjectileRow row)
        {
            ActorId = ActorIds.Projectile;
            Id = row.Id;
            Type = row.Type;
            RarityType = row.Rarity.ToRarityType();
            Durability = row.Durability;
            Attack = row.Attack;
            AttackRange = row.AttackRange;
            HitRange = row.HitRange;
            MoveSpeed = row.MoveSpeed;
            CriticalRate = row.CriticalRate;
            CriticalDamage = row.CriticalDamage;
        }
    }
}