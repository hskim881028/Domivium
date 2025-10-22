using System;
using Domivium.Client.Contents.Battle;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Battle;

namespace Domivium.Client.Contents.Actors
{
    public static class PawnTypes
    {
        public static PawnType Melee = new(0);
        public static PawnType Ranged = new(1);
        public static PawnType Tank = new(2);
        public static PawnType Support = new(3);

        public static PawnType ToPawnType(this string type)
        {
            return type switch
            {
                "Melee" => Melee,
                "Ranged" => Ranged,
                "Tank" => Tank,
                "Support" => Support,
                _ => throw new Exception($"Unknown Pawn type: {type}")
            };
        }

        public static BattleAbilityId ToBattleAbilityId(this string type)
        {
            return type switch
            {
                "Melee" or "Ranged" or "Tank" => BattleAbilityIds.Attack,
                "Support" => BattleAbilityIds.Heal,
                _ => throw new Exception($"Unknown Pawn type: {type}")
            };
        }
    }
}