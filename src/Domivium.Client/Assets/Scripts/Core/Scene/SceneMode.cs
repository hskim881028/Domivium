using UnitGenerator;

namespace Domivium.Client.Core.Scene
{
    [UnitOf(typeof(int), UnitGenerateOptions.ImplicitOperator | UnitGenerateOptions.Comparable | UnitGenerateOptions.ArithmeticOperator)]
    public readonly partial struct SceneMode
    {
        public static readonly SceneMode Loading = 0;
        public static readonly SceneMode UILoaded = 1;
        public static readonly SceneMode DataLoaded = 2;
        public static readonly SceneMode SpawnCompleted = 3;
        public static readonly SceneMode Run = 4;
        public static readonly SceneMode Pause = 5;
        public static readonly SceneMode Terminated = 6;
    }
}