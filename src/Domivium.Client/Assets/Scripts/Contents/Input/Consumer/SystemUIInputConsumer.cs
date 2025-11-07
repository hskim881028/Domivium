using System;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Input;
using Domivium.Client.Core.Message;
using Domivium.Client.Core.UI.Navigation;

namespace Domivium.Client.Contents.Input.Consumer
{
    public class SystemUIInputConsumer : IInputConsumer
    {
        private readonly InputEventSystem _inputEventSystem;
        private readonly IUINavigation _uiNavigation;

        public InputPriority Priority => InputPriorities.StackUI;

        public SystemUIInputConsumer(InputEventSystem inputEventSystem, IUINavigation uiNavigation)
        {
            _inputEventSystem = inputEventSystem;
            _uiNavigation = uiNavigation;
        }

        public bool TryHandle(InputMessage message)
        {
            switch (message.Type)
            {
                case InputMessageType.Submit:
                case InputMessageType.Cancel:
                    return _uiNavigation.HasOpenSystemUI;
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