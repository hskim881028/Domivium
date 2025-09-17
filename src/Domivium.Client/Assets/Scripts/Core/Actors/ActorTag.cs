using UnitGenerator;

namespace Domivium.Client.Core.Actors
{
    [UnitOf(typeof(int), UnitGenerateOptions.ImplicitOperator | UnitGenerateOptions.Comparable)]
    public readonly partial struct ActorTag { }
}