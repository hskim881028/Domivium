using UnitGenerator;

namespace Domivium.Client.Core.Battle
{
    [UnitOf(typeof(int))]
    public readonly partial struct BattleCueId
    {
        public static BattleCueId None = new(0);
    }
}