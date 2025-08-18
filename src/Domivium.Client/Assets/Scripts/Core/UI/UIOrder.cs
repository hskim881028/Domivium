using UnitGenerator;

namespace Domivium.Client.Core.UI
{
    [UnitOf(typeof(int), UnitGenerateOptions.ImplicitOperator | UnitGenerateOptions.Comparable | UnitGenerateOptions.ArithmeticOperator)]
    public readonly partial struct UIOrder
    {
        public static readonly UIOrder StaticId = 0;
        public static readonly UIOrder StackId = 100;
        public static readonly UIOrder SystemId = 1000;
    }
}