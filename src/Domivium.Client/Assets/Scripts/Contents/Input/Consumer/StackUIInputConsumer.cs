using System;
using Cysharp.Threading.Tasks;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Context;
using Domivium.Client.Core.Input;
using Domivium.Client.Core.Message;
using Domivium.Client.Core.UI.Contract;
using Domivium.Client.Core.UI.Navigation;

namespace Domivium.Client.Contents.Input.Consumer
{
    public sealed class StackUIInputConsumer : InputConsumer
    {
        private readonly InputEventSystem _inputEventSystem;

        public override InputPriority Priority => InputPriorities.StackUI;

        public StackUIInputConsumer(
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
                    return UINavigation.HasOpenStackUI;
                case InputMessageType.Cancel:
                    if (UINavigation.IsRunning || !UINavigation.HasOpenStackUI) return false;

                    HideStackUIAsync().Forget();
                    return true;
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

        private async UniTaskVoid HideStackUIAsync()
        {
            await UINavigation.HideStackUIAsync(UIResult.Close);

            if (!UINavigation.HasOpenStackUI)
            {
                await ApplyUILayer();
            }
        }
    }
}