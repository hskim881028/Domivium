using System;
using Domivium.Client.Core.Input;
using Domivium.Client.Core.Message;
using Domivium.Client.Core.Utility;

namespace Domivium.Client.Contents.Input.Consumer
{
    public sealed class BuildPlacementInputConsumer : IInputConsumer
    {
        public InputPriority Priority => InputPriorities.BuildPlacement;

        public bool TryHandle(InputMessage message)
        {
            switch (message.Type)
            {
                case InputMessageType.Submit:
                case InputMessageType.Cancel:
                    this.Log($"{message.Type}");
                    return true;
                case InputMessageType.Click:
                case InputMessageType.Point:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}