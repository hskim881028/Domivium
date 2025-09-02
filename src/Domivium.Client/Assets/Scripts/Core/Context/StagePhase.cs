using UnitGenerator;

namespace Domivium.Client.Core.Context
{
    [UnitOf(typeof(int), UnitGenerateOptions.ImplicitOperator | UnitGenerateOptions.Comparable | UnitGenerateOptions.ArithmeticOperator)]
    public readonly partial struct StagePhase
    {
        public static readonly StagePhase Idle = 0;
    }
}