using UnitGenerator;

namespace Domivium.Client.Data.Stat
{
    [UnitOf(typeof(int))]
    public readonly partial struct StatDomain
    {
        public static readonly StatDomain Value = new(0);
        public static readonly StatDomain Rate = new(1);
    }
}