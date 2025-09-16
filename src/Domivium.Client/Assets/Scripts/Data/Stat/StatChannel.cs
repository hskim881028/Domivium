using UnitGenerator;

namespace Domivium.Client.Data.Stat
{
    [UnitOf(typeof(int))]
    public readonly partial struct StatChannel
    {
        public static readonly StatChannel Add = new(0);
        public static readonly StatChannel PostAdd = new(1);
        public static readonly StatChannel AddMultiplier = new(2);
    }
}