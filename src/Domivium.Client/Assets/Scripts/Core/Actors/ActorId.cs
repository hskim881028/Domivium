using UnitGenerator;

namespace Domivium.Client.Core.Actors
{
    [UnitOf(typeof(int), UnitGenerateOptions.ImplicitOperator | UnitGenerateOptions.Comparable | UnitGenerateOptions.ArithmeticOperator)]
    public readonly partial struct ActorId
    {
        public static ActorId None = 0;
    }
}