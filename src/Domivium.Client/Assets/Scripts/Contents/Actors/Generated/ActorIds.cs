using System;
using Domivium.Client.Core.Actors;

namespace Domivium.Client.Contents.Actors.Generated
{
    public static class ActorIds
    {
        public static ActorId Map = 0;
        public static ActorId Nexus = 1;
        public static ActorId Tower = 2;
        public static ActorId Character = 3;
        public static ActorId Monster = 4;
        public static ActorId CharacterPathIndicator = 5;
        public static ActorId DamageText = 6;
        public static ActorId HealText = 7;

        public static ActorId ToActorId(this string type)
        {
            return type switch
            {
                "Nexus" => Nexus,
                "Tower" => Tower,
                "Character" => Character,
                "Monster" => Monster,
                _ => throw new Exception($"Unknown Unit type: {type}")
            };
        }
    }
}