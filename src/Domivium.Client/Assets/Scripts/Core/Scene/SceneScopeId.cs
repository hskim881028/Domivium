using UnitGenerator;

namespace Domivium.Client.Core.Scene
{
    [UnitOf(typeof(int), UnitGenerateOptions.ImplicitOperator | UnitGenerateOptions.Comparable | UnitGenerateOptions.ArithmeticOperator)]
    public readonly partial struct SceneScopeId { }
}