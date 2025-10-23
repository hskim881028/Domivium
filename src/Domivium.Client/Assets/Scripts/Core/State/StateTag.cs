using UnitGenerator;

namespace Domivium.Client.Core.State
{
    [UnitOf(typeof(int), UnitGenerateOptions.ImplicitOperator | UnitGenerateOptions.Comparable)]
    public readonly partial struct StateTag
    {
        public static readonly StateTag Idle = 100;
        public static readonly StateTag Die = 101;
        public static readonly StateTag Despawn = 102;
        public static readonly StateTag Terminated = 103;
    }
}