using System;
using Domivium.Client.Core.Input;
using Domivium.Client.Core.Message;
using Domivium.Client.Core.Systems;

namespace Domivium.Client.Contents.Input.Consumer
{
    public class ApplicationInputConsumer : IInputConsumer
    {
        private readonly IPointerSystemCommand _pointerSystemCommand;
        public InputPriority Priority => InputPriorities.Application;

        public ApplicationInputConsumer(IPointerSystemCommand pointerSystemCommand)
        {
            _pointerSystemCommand = pointerSystemCommand;
        }

        public bool TryHandle(InputMessage message)
        {
            switch (message.Type)
            {
                case InputMessageType.Submit:
                case InputMessageType.Cancel:
                case InputMessageType.Move:
                    return false;
                case InputMessageType.Point:
                case InputMessageType.ClickEnter:
                case InputMessageType.ClickExit:
                    _pointerSystemCommand.Update(message.Value);
                    return false;
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