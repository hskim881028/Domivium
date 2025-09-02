using System;
using Domivium.Client.Core.Input;
using Domivium.Client.Core.Message;

namespace Domivium.Client.Contents.Input.Consumer
{
    public sealed class BattleInputConsumer : IInputConsumer
    {
        public InputPriority Priority => InputPriorities.Battle;

        public bool TryHandle(InputMessage message)
        {
            switch (message.Type)
            {
                case InputMessageType.Submit:
                case InputMessageType.Cancel:
                case InputMessageType.ClickEnter:
                case InputMessageType.ClickExit:
                case InputMessageType.Point:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}