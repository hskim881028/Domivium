using UnitGenerator;

namespace Domivium.Client.Core.UI
{
    [UnitOf(typeof(int), UnitGenerateOptions.ImplicitOperator | UnitGenerateOptions.Comparable | UnitGenerateOptions.ArithmeticOperator)]
    public readonly partial struct UIId { }
}