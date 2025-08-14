using UnitGenerator;

namespace Domivium.Client.Core.Scene
{
    [UnitOf(typeof(int), UnitGenerateOptions.ImplicitOperator | UnitGenerateOptions.Comparable | UnitGenerateOptions.ArithmeticOperator)]
    public readonly partial struct SceneScopeId
    {
        public static SceneScopeId Lobby = 0;
        public static SceneScopeId Stage = 1;

        public string ToName()
        {
            if (this == Lobby) return "Lobby";

            if (this == Stage) return "Stage";

            return "Scope";
        }
    }
}