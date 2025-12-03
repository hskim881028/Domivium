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
        public static readonly StatId WeightCapacity = 6;
        public static readonly StatId InventoryCapacity = 7;
        public static readonly StatId ProjectileCapacity = 8;
        public static readonly StatId Attack = 9;
        public static readonly StatId Defense = 10;
        public static readonly StatId Penetration = 11;
        public static readonly StatId DetectionRange = 12;
        public static readonly StatId AttackRange = 13;
        public static readonly StatId MoveSpeed = 14;
        public static readonly StatId AttackSpeed = 15;
        public static readonly StatId ReloadSpeed = 16;
        public static readonly StatId ProjectileSpeed = 17;
        public static readonly StatId CriticalRate = 18;
        public static readonly StatId CriticalDamage = 19;
        public static readonly StatId StatCount = 20;
    }
}