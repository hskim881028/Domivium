using UnitGenerator;

namespace Domivium.Client.Core.Systems
{
    [UnitOf(typeof(int), UnitGenerateOptions.ImplicitOperator | UnitGenerateOptions.Comparable | UnitGenerateOptions.ArithmeticOperator)]
    public readonly partial struct StageMode
    {
        public static readonly StageMode Prepare = 0;
        public static readonly StageMode Run = 1;
        public static readonly StageMode Pause = 2;
        public static readonly StageMode Terminated = 3;
    }
}