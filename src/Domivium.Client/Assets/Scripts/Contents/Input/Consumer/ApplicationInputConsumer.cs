using System;
using Domivium.Client.Core.Context;
using Domivium.Client.Core.Input;
using Domivium.Client.Core.Message;
using Domivium.Client.Core.Systems;
using Domivium.Client.Core.UI.Navigation;

namespace Domivium.Client.Contents.Input.Consumer
{
    public class ApplicationInputConsumer : InputConsumer
    {
        private readonly IPointerSystemCommand _pointerSystemCommand;

        public override InputPriority Priority => InputPriorities.Application;

        public ApplicationInputConsumer(
            IAppContext appContext,
            IUINavigation uiNavigation,
            IPointerSystemCommand pointerSystemCommand)
            : base(appContext, uiNavigation)
        {
            _pointerSystemCommand = pointerSystemCommand;
        }

        public override bool TryHandle(InputMessage message)
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