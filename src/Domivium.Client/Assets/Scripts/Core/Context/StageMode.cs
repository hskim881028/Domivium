using UnitGenerator;

namespace Domivium.Client.Core.Context
{
    [UnitOf(typeof(int), UnitGenerateOptions.ImplicitOperator | UnitGenerateOptions.Comparable | UnitGenerateOptions.ArithmeticOperator)]
    public readonly partial struct StageMode
    {
        public static readonly StageMode Idle = 0;
    }
}