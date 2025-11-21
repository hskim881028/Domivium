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
        public static readonly StatId InventoryCapacity = 6;
        public static readonly StatId ProjectileCapacity = 7;
        public static readonly StatId Attack = 8;
        public static readonly StatId Defense = 9;
        public static readonly StatId DetectionRange = 10;
        public static readonly StatId AttackRange = 11;
        public static readonly StatId MoveSpeed = 12;
        public static readonly StatId AttackSpeed = 13;
        public static readonly StatId ReloadSpeed = 14;
        public static readonly StatId ProjectileSpeed = 15;
        public static readonly StatId CriticalRate = 16;
        public static readonly StatId CriticalDamage = 17;
        public static readonly StatId StatCount = 18;
    }
}