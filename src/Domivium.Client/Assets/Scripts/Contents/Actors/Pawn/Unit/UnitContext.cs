using Domivium.Client.Contents.Actors;
using Domivium.Client.Contents.Actors.Generated;
using Domivium.Client.Core.Actors;
using Domivium.Client.Data.Row;

namespace Domivium.Client.Core.Battle
{
    public readonly struct UnitContext
    {
        public int Id { get; }
        public int Health { get; }
        public int Attack { get; }
        public int Defense { get; }
        public int MoveSpeed { get; } // percent
        public int AttackSpeed { get; } // percent
        public int HitRange { get; } // percent
        public int AttackRange { get; } // percent
        public int DetectionRange { get; } // percent
        public int CriticalRate { get; } // percent
        public int CriticalDamage { get; } // percent

        public ActorId ActorId { get; }
        public ActorId TargetActionId { get; }
        public BattleAbilityId BattleAbilityId { get; }
        public UnitType UnitType { get; }

        public UnitContext(CharacterRow row)
        {
            Id = row.Id;
            Health = row.Health;
            Attack = row.Attack;
            Defense = row.Defense;
            MoveSpeed = row.MoveSpeed;
            AttackSpeed = row.AttackSpeed;
            HitRange = row.HitRange;
            AttackRange = row.AttackRange;
            DetectionRange = row.DetectionRange;
            CriticalRate = row.CriticalRate;
            CriticalDamage = row.CriticalDamage;

            ActorId = ActorIds.Character;
            TargetActionId = row.Target.ToActorId();
            BattleAbilityId = row.Job.ToBattleAbilityId();
            UnitType = row.Job.ToUnitType();
        }

        public UnitContext(MonsterRow row)
        {
            Id = row.Id;
            Health = row.Health;
            Attack = row.Attack;
            Defense = row.Defense;
            MoveSpeed = row.MoveSpeed;
            AttackSpeed = row.AttackSpeed;
            HitRange = row.HitRange;
            AttackRange = row.AttackRange;
            DetectionRange = row.DetectionRange;
            CriticalRate = row.CriticalRate;
            CriticalDamage = row.CriticalDamage;

            ActorId = ActorIds.Monster;
            TargetActionId = row.Target.ToActorId();
            BattleAbilityId = row.Job.ToBattleAbilityId();
            UnitType = row.Job.ToUnitType();
        }

        public UnitContext(TowerRow row)
        {
            Id = row.Id;
            Health = row.Health;
            Attack = row.Attack;
            Defense = row.Defense;
            MoveSpeed = row.MoveSpeed;
            AttackSpeed = row.AttackSpeed;
            AttackRange = row.AttackRange;
            HitRange = row.HitRange;
            DetectionRange = row.DetectionRange;
            CriticalRate = row.CriticalRate;
            CriticalDamage = row.CriticalDamage;

            ActorId = ActorIds.Tower;
            TargetActionId = row.Target.ToActorId();
            BattleAbilityId = row.Job.ToBattleAbilityId();
            UnitType = row.Job.ToUnitType();
        }

        public UnitContext(NexusRow row)
        {
            Id = row.Id;
            Health = row.Health;
            Attack = row.Attack;
            Defense = row.Defense;
            MoveSpeed = row.MoveSpeed;
            AttackSpeed = row.AttackSpeed;
            HitRange = row.HitRange;
            AttackRange = row.AttackRange;
            DetectionRange = row.DetectionRange;
            CriticalRate = row.CriticalRate;
            CriticalDamage = row.CriticalDamage;

            ActorId = ActorIds.Nexus;
            TargetActionId = row.Target.ToActorId();
            BattleAbilityId = row.Job.ToBattleAbilityId();
            UnitType = row.Job.ToUnitType();
        }
    }
}