using Domivium.Client.Core.Context;

namespace Domivium.Client.Contents.Context
{
    public static class StageModes
    {
        public static readonly StageMode Idle = StageMode.Idle;
        public static readonly StageMode Battle = 1;
        public static readonly StageMode TowerPlacement = 2;

        public static string ToName(this StageMode mode) => mode.AsPrimitive() switch
        {
            0 => nameof(Idle),
            1 => nameof(Battle),
            2 => nameof(TowerPlacement),
            _ => mode.AsPrimitive().ToString()
        };
    }
}