using System.Collections.Generic;
using UnitGenerator;

namespace Domivium.Client.Core.Actors
{
    [UnitOf(typeof(int), UnitGenerateOptions.ImplicitOperator | UnitGenerateOptions.Comparable)]
    public readonly partial struct ActorTag
    {
        public static readonly ActorTag None = 0;
        public static readonly ActorTag Idle = 1;
        public static readonly ActorTag Hit = 2;
        public static readonly ActorTag Die = 3;
        public static readonly ActorTag Despawn = 4;

        private static readonly HashSet<ActorTag> EmptyInternal = new();
        private static readonly HashSet<ActorTag> OnlyDieInternal = new() { Die };
        public static IReadOnlyCollection<ActorTag> Empty => EmptyInternal;
        public static IReadOnlyCollection<ActorTag> OnlyDie => OnlyDieInternal;
        public static HashSet<ActorTag> Set(params ActorTag[] values) => new(values);
    }
}