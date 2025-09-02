using System;
using Domivium.Client.Contents.Commands;
using Domivium.Client.Core.Input;
using Domivium.Client.Core.Message;

namespace Domivium.Client.Contents.Input.Consumer
{
    public class ApplicationInputConsumer : IInputConsumer
    {
        private readonly IPointerCommand _pointerCommand;
        public InputPriority Priority => InputPriorities.Application;

        public ApplicationInputConsumer(IPointerCommand pointerCommand)
        {
            _pointerCommand = pointerCommand;
        }

        public bool TryHandle(InputMessage message)
        {
            switch (message.Type)
            {
                case InputMessageType.Cancel:
                case InputMessageType.Submit:
                    return false;
                case InputMessageType.Point:
                case InputMessageType.Click:
                    _pointerCommand.Update(message.Value);
                    return false;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}