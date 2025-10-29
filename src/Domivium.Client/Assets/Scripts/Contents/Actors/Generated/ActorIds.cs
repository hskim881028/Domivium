using System;
using Domivium.Client.Core.Actors;

namespace Domivium.Client.Contents.Actors.Generated
{
    public static class ActorIds
    {
        public static ActorId StageField = 1;
        public static ActorId CharacterCamp = 2;
        public static ActorId MonsterCamp = 3;
        public static ActorId Character = 6;
        public static ActorId Monster = 7;
        public static ActorId DamageText = 10;
        public static ActorId HealText = 11;
        public static ActorId SoulEffect = 12;
        public static ActorId SlashEffect = 13;
        public static ActorId ProjectileEffect = 14;

        public static ActorId ToActorId(this string type)
        {
            return type switch
            {
                "Character" => Character,
                "Monster" => Monster,
                _ => throw new Exception($"Unknown Unit type: {type}")
            };
        }

        public static ActorId FromCampTypeToActorId(this string type)
        {
            return type switch
            {
                "CharacterCamp" => CharacterCamp,
                "MonsterCamp" => MonsterCamp,
                _ => throw new Exception($"Unknown Camp type: {type}")
            };
        }
    }
}