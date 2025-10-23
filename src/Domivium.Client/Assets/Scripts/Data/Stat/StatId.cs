using UnitGenerator;

namespace Domivium.Client.Data.Stat
{
    [UnitOf(typeof(int), UnitGenerateOptions.ImplicitOperator | UnitGenerateOptions.Comparable | UnitGenerateOptions.ArithmeticOperator)]
    public readonly partial struct StatId
    {
        public static readonly StatId Level = 0;
        public static readonly StatId Health = 1;
        public static readonly StatId Attack = 2;
        public static readonly StatId Defense = 3;
        public static readonly StatId MoveSpeed = 4;
        public static readonly StatId AttackSpeed = 5;
        public static readonly StatId HitRange = 6;
        public static readonly StatId AttackRange = 7;
        public static readonly StatId DetectionRange = 8;
        public static readonly StatId CriticalRate = 9;
        public static readonly StatId CriticalDamage = 10;
        public static readonly StatId StatCount = 11;
    }
}