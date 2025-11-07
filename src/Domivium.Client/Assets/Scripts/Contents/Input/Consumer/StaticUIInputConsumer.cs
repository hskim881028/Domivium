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
                case InputMessageType.Submit:
                    return false;
                case InputMessageType.Cancel:
                    _inputEventSystem.ReleaseSelectedGameObject();
                    return false;
                case InputMessageType.ClickEnter:
                case InputMessageType.ClickExit:
                    return !_inputEventSystem.BlockUI && _inputEventSystem.IsPointerOverUI(message.Value);
                case InputMessageType.Point:
                case InputMessageType.Move:
                case InputMessageType.Look:
                case InputMessageType.LookCanceled:
                case InputMessageType.Quick:
                case InputMessageType.QuickCanceled:
                case InputMessageType.Inventory:
                case InputMessageType.Interact:
                case InputMessageType.Avoid:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}