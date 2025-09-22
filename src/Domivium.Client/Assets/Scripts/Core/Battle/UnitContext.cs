using Domivium.Client.Data.Row;

namespace Domivium.Client.Core.Battle
{
    public readonly struct UnitContext
    {
        public int Id { get; }
        public string Job { get; }
        public string Target { get; }
        public int Health { get; }
        public int Attack { get; }
        public int Defense { get; }
        public int HitRange { get; }
        public int AttackRange { get; }
        public int DetectionRange { get; } // percent
        public int Speed { get; } // percent
        public int CriticalRate { get; } // percent
        public int CriticalDamage { get; } // percent

        public UnitContext(CharacterRow row)
        {
            Id = row.Id;
            Job = row.Job;
            Target = row.Target;
            Health = row.Health;
            Attack = row.Attack;
            Defense = row.Defense;
            HitRange = row.HitRange;
            AttackRange = row.AttackRange;
            DetectionRange = row.DetectionRange;
            Speed = row.Speed;
            CriticalRate = row.CriticalRate;
            CriticalDamage = row.CriticalDamage;
        }

        public UnitContext(MonsterRow row)
        {
            Id = row.Id;
            Job = row.Job;
            Target = row.Target;
            Health = row.Health;
            Attack = row.Attack;
            Defense = row.Defense;
            HitRange = row.HitRange;
            AttackRange = row.AttackRange;
            DetectionRange = row.DetectionRange;
            Speed = row.Speed;
            CriticalRate = row.CriticalRate;
            CriticalDamage = row.CriticalDamage;
        }

        public UnitContext(TowerRow row)
        {
            Id = row.Id;
            Job = row.Job;
            Target = row.Target;
            Health = row.Health;
            Attack = row.Attack;
            Defense = row.Defense;
            AttackRange = row.AttackRange;
            HitRange = row.HitRange;
            DetectionRange = row.DetectionRange;
            Speed = row.Speed;
            CriticalRate = row.CriticalRate;
            CriticalDamage = row.CriticalDamage;
        }

        public UnitContext(NexusRow row)
        {
            Id = row.Id;
            Job = row.Job;
            Target = row.Target;
            Health = row.Health;
            Attack = row.Attack;
            Defense = row.Defense;
            HitRange = row.HitRange;
            AttackRange = row.AttackRange;
            DetectionRange = row.DetectionRange;
            Speed = row.Speed;
            CriticalRate = row.CriticalRate;
            CriticalDamage = row.CriticalDamage;
        }
    }
}