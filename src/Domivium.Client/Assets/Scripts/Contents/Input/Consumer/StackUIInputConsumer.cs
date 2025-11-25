using System;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.DI;
using Domivium.Client.Contents.UI;
using Domivium.Client.Core;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Input;
using Domivium.Client.Core.Message;
using Domivium.Client.Core.UI.Contract;
using Domivium.Client.Core.UI.Navigation;

namespace Domivium.Client.Contents.Input.Consumer
{
    public sealed class StackUIInputConsumer : Disposable, IInputConsumer
    {
        private readonly IAppContext _appContext;
        private readonly InputEventSystem _inputEventSystem;
        private readonly IUINavigation _uiNavigation;

        public InputPriority Priority => InputPriorities.StackUI;

        public StackUIInputConsumer(
            IAppContext appContext,
            InputEventSystem inputEventSystem,
            IUINavigation uiNavigation)
        {
            _appContext = appContext;
            _inputEventSystem = inputEventSystem;
            _uiNavigation = uiNavigation;
        }

        public bool TryHandle(InputMessage message)
        {
            switch (message.Type)
            {
                case InputMessageType.Submit:
                    return _uiNavigation.HasOpenStackUI;
                case InputMessageType.Cancel:
                    if (_uiNavigation.IsRunning || !_uiNavigation.HasOpenStackUI) return false;

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
            await _uiNavigation.HideStackUIAsync(UIResult.Close);

            if (!_uiNavigation.HasOpenStackUI)
            {
                var layer = UILayers.HideAll;
                if (_appContext.Scene.CurrentValue == SceneScopeIds.Title)
                {
                    layer = UILayers.Title;
                }
                else if (_appContext.Scene.CurrentValue == SceneScopeIds.Lobby)
                {
                    layer = UILayers.Lobby;
                }
                else if (_appContext.Scene.CurrentValue == SceneScopeIds.Stage)
                {
                    layer = UILayers.Stage;
                }

                await _uiNavigation.ApplyUILayer(layer);
            }
        }
    }
}