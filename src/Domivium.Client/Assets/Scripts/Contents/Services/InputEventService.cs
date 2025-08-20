using Domivium.Client.Core.Actor;

namespace Domivium.Client.Contents.Services
{
    public sealed class InputEventService
    {
        private readonly InputEventSystem _inputEventSystem;

        public InputEventService(InputEventSystem inputEventSystem)
        {
            _inputEventSystem = inputEventSystem;
        }
    }
}