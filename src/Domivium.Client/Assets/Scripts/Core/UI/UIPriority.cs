using UnitGenerator;

namespace Domivium.Client.Core.UI
{
    [UnitOf(typeof(int), UnitGenerateOptions.ImplicitOperator | UnitGenerateOptions.Comparable | UnitGenerateOptions.ArithmeticOperator)]
    public readonly partial struct UIPriority
    {
        public static readonly UIPriority Default = 0;
    }
}