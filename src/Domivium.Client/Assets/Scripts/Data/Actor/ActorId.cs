using UnitGenerator;

namespace Domivium.Client.Core.Actors
{
    [UnitOf(typeof(int))]
    public readonly partial struct ActorId
    {
        public static ActorId None = new(0);
        public static ActorId Character = new(1);
        public static ActorId Monster = new(2);
        public static ActorId Projectile = new(3);
        public static ActorId Prop = new(4);

        public static ActorId LobbyField = new(10);
        public static ActorId StageField = new(11);

        public static ActorId DamageText = new(100);
        public static ActorId HealText = new(101);
    }
}