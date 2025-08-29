using Domivium.Client.Core.Input;

namespace Domivium.Client.Contents.Input
{
    public static class InputPriorities
    {
        public static InputPriority SystemUI = 0;
        public static InputPriority StackUI = 1;
        public static InputPriority StaticUI = 2;
        public static InputPriority BuildPlacement = 3;
        public static InputPriority Gameplay = 4;
    }
}