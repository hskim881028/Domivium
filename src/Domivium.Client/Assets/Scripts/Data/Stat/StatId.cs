using UnitGenerator;

namespace Domivium.Client.Data.Stat
{
    [UnitOf(typeof(int), UnitGenerateOptions.ImplicitOperator | UnitGenerateOptions.Comparable | UnitGenerateOptions.ArithmeticOperator)]
    public readonly partial struct StatId
    {
        public static readonly StatId Health = 0;
        public static readonly StatId Hunger = 1;
        public static readonly StatId Stamina = 2;
        public static readonly StatId Sanity = 3;
        public static readonly StatId Durability = 4;
        public static readonly StatId Weight = 5;
        public static readonly StatId MaxWeight = 6;
        public static readonly StatId InventoryCapacity = 7;
        public static readonly StatId ProjectileCapacity = 8;
        public static readonly StatId Attack = 9;
        public static readonly StatId Defense = 10;
        public static readonly StatId AttackRange = 11;
        public static readonly StatId HitRange = 12;
        public static readonly StatId DetectionRange = 13;
        public static readonly StatId FieldOfView = 14;
        public static readonly StatId AngleOfView = 15;
        public static readonly StatId MoveSpeed = 16;
        public static readonly StatId AttackSpeed = 17;
        public static readonly StatId ReloadSpeed = 18;
        public static readonly StatId CriticalRate = 19;
        public static readonly StatId CriticalDamage = 20;
        public static readonly StatId StatCount = 21;
    }
}