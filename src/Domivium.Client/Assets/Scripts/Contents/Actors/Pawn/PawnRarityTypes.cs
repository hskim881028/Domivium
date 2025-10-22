using System;
using Domivium.Client.Core.Actors;

namespace Domivium.Client.Contents.Actors
{
    public static class PawnRarityTypes
    {
        public static PawnRarityType Common = new(0);
        public static PawnRarityType Uncommon = new(1);
        public static PawnRarityType Rare = new(2);
        public static PawnRarityType Epic = new(3);
        public static PawnRarityType Legendary = new(4);
        public static PawnRarityType Mythic = new(5);
        public static PawnRarityType Unique = new(6);
        public static PawnRarityType Event = new(7);
        
        public static PawnRarityType ToPawnRarityType(this string type)
        {
            return type switch
            {
                "Common" => Common,
                "Uncommon" => Uncommon,
                "Rare" => Rare,
                "Epic" => Epic,
                "Legendary" => Legendary,
                "Mythic" => Mythic,
                "Unique" => Unique,
                "Event" => Event,
                _ => throw new Exception($"Unknown Pawn Rarity type: {type}")
            };
        }
    }
}