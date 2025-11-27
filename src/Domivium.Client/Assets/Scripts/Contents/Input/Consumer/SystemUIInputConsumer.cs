using System;
using Domivium.Client.Core;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Input;
using Domivium.Client.Core.Message;
using Domivium.Client.Core.UI.Navigation;

namespace Domivium.Client.Contents.Input.Consumer
{
    public class SystemUIInputConsumer : InputConsumer
    {
        private readonly InputEventSystem _inputEventSystem;

        public override InputPriority Priority => InputPriorities.StackUI;

        public SystemUIInputConsumer(
            IAppContext appContext,
            IUINavigation uiNavigation,
            InputEventSystem inputEventSystem)
            : base(appContext, uiNavigation)
        {
            _inputEventSystem = inputEventSystem;
        }

        public override bool TryHandle(InputMessage message)
        {
            switch (message.Type)
            {
                case InputMessageType.Submit:
                case InputMessageType.Cancel:
                    return UINavigation.HasOpenSystemUI;
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