using System;
using Domivium.Client.Core.Actors;

namespace Domivium.Client.Contents.Actors
{
    public static class RarityTypes
    {
        public static RarityType Common = new(0);
        public static RarityType Uncommon = new(1);
        public static RarityType Rare = new(2);
        public static RarityType Epic = new(3);
        public static RarityType Legendary = new(4);
        public static RarityType Mythic = new(5);
        public static RarityType Unique = new(6);
        public static RarityType Event = new(7);

        public static RarityType ToRarityType(this string type)
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