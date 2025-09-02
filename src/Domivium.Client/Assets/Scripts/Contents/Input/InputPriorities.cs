using Domivium.Client.Core.Input;

namespace Domivium.Client.Contents.Input
{
    public static class InputPriorities
    {
        public static InputPriority Application = 0;
        public static InputPriority SystemUI = 1;
        public static InputPriority StackUI = 2;
        public static InputPriority StaticUI = 3;
        public static InputPriority TowerPlacement = 4;
        public static InputPriority Gameplay = 5;
    }
}