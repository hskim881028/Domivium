using System;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Input;
using Domivium.Client.Core.Message;
using Domivium.Client.Core.UI.Navigation;
using Domivium.Client.Core.Utility;

namespace Domivium.Client.Contents.Input.Consumer
{
    public sealed class StackUIInputConsumer : IInputConsumer
    {
        private readonly InputEventSystem _inputEventSystem;
        private readonly IUINavigation _uiNavigation;

        public StackUIInputConsumer(InputEventSystem inputEventSystem, IUINavigation uiNavigation)
        {
            _inputEventSystem = inputEventSystem;
            _uiNavigation = uiNavigation;
        }

        public InputPriority Priority => InputPriorities.StackUI;

        public bool TryHandle(InputMessage message)
        {
            switch (message.Type)
            {
                case InputMessageType.Submit:
                case InputMessageType.Cancel:
                    this.Log();
                    return _uiNavigation.HasOpenStackUI;
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