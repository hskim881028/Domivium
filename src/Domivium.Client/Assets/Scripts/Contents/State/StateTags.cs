using System;
using System.Collections.Generic;
using Domivium.Client.Core.State;

namespace Domivium.Client.Contents.State
{
    public static class StateTags
    {
        public static readonly StateTag Spawn = 1;
        public static readonly StateTag Idle = 2;
        public static readonly StateTag Die = 3;
        public static readonly StateTag Despawn = 4;
        public static readonly StateTag Move = 5;
        public static readonly StateTag Chase = 6;
        public static readonly StateTag Battle = 7;
        public static readonly StateTag Stun = 8;
        public static readonly StateTag Invincible = 9;
        public static readonly StateTag Terminated = 99;

        private static readonly HashSet<StateTag> EmptyInternal = new();
        private static readonly HashSet<StateTag> DefaultBlockedTagInternal = new() { Die, Despawn };
        public static IReadOnlyCollection<StateTag> Empty => EmptyInternal;
        public static IReadOnlyCollection<StateTag> DefaultBlockedTag => DefaultBlockedTagInternal;
        public static HashSet<StateTag> Set(params StateTag[] values) => new(values);

        public static string ToState(this StateTag tag)
        {
            return tag.AsPrimitive() switch
            {
                1 => "Spawn",
                2 => "Idle",
                3 => "Die",
                4 => "Despawn",
                5 => "Move",
                6 => "Chase",
                7 => "Battle",
                8 => "Stun",
                9 => "Invincible",
                99 => "GameEnd",
                _ => throw new Exception($"Unknown ActorTag: {tag}")
            };
        }
    }
}