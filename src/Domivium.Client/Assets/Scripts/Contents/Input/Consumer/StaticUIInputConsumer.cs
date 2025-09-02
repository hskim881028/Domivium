using System;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Input;
using Domivium.Client.Core.Message;

namespace Domivium.Client.Contents.Input.Consumer
{
    public sealed class StaticUIInputConsumer : IInputConsumer
    {
        private readonly InputEventSystem _inputEventSystem;

        public InputPriority Priority => InputPriorities.StackUI;

        public StaticUIInputConsumer(InputEventSystem inputEventSystem)
        {
            _inputEventSystem = inputEventSystem;
        }

        public bool TryHandle(InputMessage message)
        {
            switch (message.Type)
            {
                case InputMessageType.Cancel:
                    _inputEventSystem.ReleaseSelectedGameObject();
                    return false;
                case InputMessageType.Submit:
                case InputMessageType.Point:
                    return false;
                case InputMessageType.ClickEnter:
                case InputMessageType.ClickExit:
                    return _inputEventSystem.IsPointerOverUI(message.Value);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}