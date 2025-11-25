using UnitGenerator;

namespace Domivium.Client.Core.Systems
{
    [UnitOf(typeof(int), UnitGenerateOptions.ImplicitOperator | UnitGenerateOptions.Comparable | UnitGenerateOptions.ArithmeticOperator)]
    public readonly partial struct StageMode
    {
        public static readonly StageMode Loading = 0;
        public static readonly StageMode UILoaded = 1;
        public static readonly StageMode DataLoaded = 2;
        public static readonly StageMode SpawnCompleted = 3;
        public static readonly StageMode Run = 4;
        public static readonly StageMode Pause = 5;
        public static readonly StageMode Terminated = 6;
    }
}