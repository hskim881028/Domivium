using UnitGenerator;

namespace Domivium.Client.Data.Stat
{
    [UnitOf(typeof(int))]
    public readonly partial struct GaugeChannel
    {
        public static readonly GaugeChannel Set = new(0);
        public static readonly GaugeChannel Add = new(1);
        public static readonly GaugeChannel Max = new(2);
        public static readonly GaugeChannel Empty = new(3);
    }
}