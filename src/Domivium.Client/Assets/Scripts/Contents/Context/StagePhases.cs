using Domivium.Client.Core.Context;

namespace Domivium.Client.Contents.Context
{
    public static class StagePhases
    {
        public static readonly StagePhase Idle = StagePhase.Idle;
        public static readonly StagePhase Paused = 1;
        public static readonly StagePhase Loading = 2;
        public static readonly StagePhase PreparingWave = 3;
        public static readonly StagePhase RunningWave = 4;
        public static readonly StagePhase Cleared = 5;
        public static readonly StagePhase Failed = 6;

        public static string ToName(this StagePhase mode) => mode.AsPrimitive() switch
        {
            0 => nameof(Idle),
            1 => nameof(Paused),
            2 => nameof(Loading),
            3 => nameof(PreparingWave),
            4 => nameof(RunningWave),
            5 => nameof(Cleared),
            6 => nameof(Failed),
            _ => mode.AsPrimitive().ToString()
        };
    }
}