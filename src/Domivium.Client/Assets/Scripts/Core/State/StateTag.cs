using UnitGenerator;

namespace Domivium.Client.Core.State
{
    [UnitOf(typeof(int), UnitGenerateOptions.ImplicitOperator | UnitGenerateOptions.Comparable)]
    public readonly partial struct StateTag { }
}