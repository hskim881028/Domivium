using UnitGenerator;

namespace Domivium.Client.Core.Battle
{
    [UnitOf(typeof(int), UnitGenerateOptions.ImplicitOperator | UnitGenerateOptions.Comparable)]
    public readonly partial struct BattleEffectTag { }
}