using System;
using Domivium.Client.Core.Actors.Unit;

namespace Domivium.Client.Contents.Actors
{
    public static class UnitTypes
    {
        public static UnitType Melee = new(0);
        public static UnitType Ranged = new(1);
        public static UnitType Tank = new(2);
        public static UnitType Support = new(3);

        public static UnitType ToUnitType(this string type)
        {
            return type switch
            {
                "Melee" => Melee,
                "Ranged" => Ranged,
                "Tank" => Tank,
                "Support" => Support,
                _ => throw new Exception($"Unknown Unit type: {type}")
            };
        }
    }
}