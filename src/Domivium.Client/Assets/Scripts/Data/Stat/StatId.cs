using UnitGenerator;

namespace Domivium.Client.Data.Stat
{
    [UnitOf(typeof(int), UnitGenerateOptions.ImplicitOperator | UnitGenerateOptions.Comparable | UnitGenerateOptions.ArithmeticOperator)]
    public readonly partial struct StatId
    {
        public static readonly StatId Health = 0;
        public static readonly StatId Attack = 1;
        public static readonly StatId Defense = 2;
        public static readonly StatId AttackRange = 3;
        public static readonly StatId HitRange = 4;
        public static readonly StatId DetectionRange = 5;
        public static readonly StatId Speed = 6;
        public static readonly StatId CriticalRate = 7;
        public static readonly StatId CriticalDamage = 8;
        public static readonly StatId StatCount = 9;
    }
}