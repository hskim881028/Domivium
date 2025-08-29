using System;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Input;
using Domivium.Client.Core.Message;
using Domivium.Client.Core.Utility;

namespace Domivium.Client.Contents.Input.Consumer
{
    public sealed class StaticUIInputConsumer : IInputConsumer
    {
        private readonly InputEventSystem _inputEventSystem;

        public StaticUIInputConsumer(InputEventSystem inputEventSystem)
        {
            _inputEventSystem = inputEventSystem;
        }

        public InputPriority Priority => InputPriorities.StackUI;

        public bool TryHandle(InputMessage message)
        {
            switch (message.Type)
            {
                case InputMessageType.Submit:
                case InputMessageType.Cancel:
                    this.Log();
                    return false;
                case InputMessageType.Click:
                    return _inputEventSystem.IsPointerOverUI(message.Value);
                case InputMessageType.Point:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}