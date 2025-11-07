using UnitGenerator;

namespace Domivium.Client.Core.Actors
{
    [UnitOf(typeof(int))]
    public readonly partial struct ActorId
    {
        public static ActorId None = new(0);
    }
}