using System;
using Domivium.Client.Core.State;

namespace Domivium.Client.Contents.State
{
    public static class StateTags
    {
        public static readonly StateTag Idle = StateTag.Idle;
        public static readonly StateTag Die = StateTag.Die;
        public static readonly StateTag Despawn = StateTag.Despawn;
        public static readonly StateTag Terminated = StateTag.Terminated;

        public static readonly StateTag Move = 0;
        public static readonly StateTag Chase = 1;
        public static readonly StateTag Battle = 2;
        public static readonly StateTag Stun = 3;
        public static readonly StateTag Invincible = 4;

        public static string ToState(this StateTag tag)
        {
            return tag.AsPrimitive() switch
            {
                100 => "Idle",
                101 => "Die",
                102 => "Despawn",
                103 => "Terminated",
                0 => "Move",
                1 => "Chase",
                2 => "Battle",
                3 => "Stun",
                4 => "Invincible",
                _ => throw new Exception($"Unknown ActorTag: {tag}")
            };
        }
    }
}