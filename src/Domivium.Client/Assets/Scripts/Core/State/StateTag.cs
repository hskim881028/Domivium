using UnitGenerator;

namespace Domivium.Client.Core.State
{
    [UnitOf(typeof(int), UnitGenerateOptions.ImplicitOperator | UnitGenerateOptions.Comparable)]
    public readonly partial struct StateTag
    {
        public static readonly StateTag Spawn = 0;
        public static readonly StateTag Idle = 1;
        public static readonly StateTag Move = 2;
        public static readonly StateTag Die = 3;
        public static readonly StateTag Despawn = 4;
        public static readonly StateTag Terminated = 5;
    }
}