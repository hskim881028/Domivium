using Domivium.Client.Contents.Actors;
using Domivium.Client.Contents.Actors.Generated;
using Domivium.Client.Core.Actors;
using Domivium.Client.Data.Row;

namespace Domivium.Client.Core.Battle
{
    public readonly struct PawnContext
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
        public PawnType PawnType { get; }
        public PawnRarityType PawnRarityType { get; }

        public PawnContext(CharacterRow row)
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
            PawnType = row.Job.ToPawnType();
            PawnRarityType = row.Rarity.ToPawnRarityType();
        }

        public PawnContext(MonsterRow row)
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
            PawnType = row.Job.ToPawnType();
            PawnRarityType = row.Rarity.ToPawnRarityType();
        }
    }
}