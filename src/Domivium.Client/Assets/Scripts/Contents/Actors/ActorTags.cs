using System.Collections.Generic;
using Domivium.Client.Core.Actors;

namespace Domivium.Client.Contents.Actors
{
    public static class ActorTags
    {
        private static readonly HashSet<ActorTag> EmptyInternal = new();
        private static readonly HashSet<ActorTag> DefaultBlockedTagInternal = new() { StateTag.Die, StateTag.Despawn };
        public static IReadOnlyCollection<ActorTag> Empty => EmptyInternal;
        public static IReadOnlyCollection<ActorTag> DefaultBlockedTag => DefaultBlockedTagInternal;
        public static HashSet<ActorTag> Set(params ActorTag[] values) => new(values);
    }

    public static class StateTag
    {
        public static readonly ActorTag Spawn = 1;
        public static readonly ActorTag Idle = 2;
        public static readonly ActorTag Die = 3;
        public static readonly ActorTag Despawn = 4;
        public static readonly ActorTag Move = 5;
        public static readonly ActorTag FindTarget = 6;
        public static readonly ActorTag Stun = 7;
        public static readonly ActorTag Invincible = 8;
    }

    public static class ActionTag
    {
        public static readonly ActorTag MeleeAttack = 101;
        public static readonly ActorTag RangedAttack = 102;
        public static readonly ActorTag Damaged = 103;
    }

    public static class BuffTag { }

    public static class DebuffTag { }
}