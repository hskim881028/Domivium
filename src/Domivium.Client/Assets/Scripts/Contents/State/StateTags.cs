using Domivium.Client.Core.State;

namespace Domivium.Client.Contents.State
{
    public static class StateTags
    {
        public static readonly StateTag Spawn = StateTag.Spawn;
        public static readonly StateTag Idle = StateTag.Idle;
        public static readonly StateTag Move = StateTag.Move;
        public static readonly StateTag Die = StateTag.Die;
        public static readonly StateTag Despawn = StateTag.Despawn;
        public static readonly StateTag Terminated = StateTag.Terminated;

        public static readonly StateTag Stun = 100;
        public static readonly StateTag Invincible = 101;
    }
}