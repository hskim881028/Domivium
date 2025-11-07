using UnitGenerator;

namespace Domivium.Client.Data.StageField
{
    [UnitOf(typeof(int), UnitGenerateOptions.ImplicitOperator | UnitGenerateOptions.Comparable)]
    public readonly partial struct StageCellTag
    {
        public static readonly StageCellTag Blocked = 0;
        public static readonly StageCellTag Occupiable = 1;
        public static readonly StageCellTag Upgradeable = 2;
    }
}