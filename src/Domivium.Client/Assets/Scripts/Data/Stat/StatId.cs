using UnitGenerator;

namespace Domivium.Client.Data.Stat
{
    [UnitOf(typeof(int), UnitGenerateOptions.ImplicitOperator | UnitGenerateOptions.Comparable | UnitGenerateOptions.ArithmeticOperator)]
    public readonly partial struct StatId
    {
        public static readonly StatId Health = 0;
        public static readonly StatId Attack = 1;
        public static readonly StatId Defense = 2;
        public static readonly StatId MoveSpeed = 3;
        public static readonly StatId AttackSpeed = 4;
        public static readonly StatId HitRange = 5;
        public static readonly StatId AttackRange = 6;
        public static readonly StatId DetectionRange = 7;
        public static readonly StatId CriticalRate = 8;
        public static readonly StatId CriticalDamage = 9;
        public static readonly StatId StatCount = 10;
    }
}