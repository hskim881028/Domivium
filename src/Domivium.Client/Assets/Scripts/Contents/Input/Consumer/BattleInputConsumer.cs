using System;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.DI;
using Domivium.Client.Contents.Services;
using Domivium.Client.Contents.UI;
using Domivium.Client.Contents.UI.Contract;
using Domivium.Client.Contents.UI.Generated;
using Domivium.Client.Core;
using Domivium.Client.Core.Input;
using Domivium.Client.Core.Message;
using Domivium.Client.Core.Systems;
using Domivium.Client.Core.UI.Contract;
using Domivium.Client.Core.UI.Navigation;
using UnityEngine;

namespace Domivium.Client.Contents.Input.Consumer
{
    public sealed class BattleInputConsumer : IInputConsumer
    {
        private readonly SceneService _sceneService;
        private readonly IAppContext _appContext;
        private readonly IUINavigation _uiNavigation;
        private readonly ICharacterSystemCommand _characterSystemCommand;

        public BattleInputConsumer(
            SceneService sceneService,
            IAppContext appContext,
            IUINavigation uiNavigation,
            ICharacterSystemCommand characterSystemCommand)
        {
            _sceneService = sceneService;
            _appContext = appContext;
            _uiNavigation = uiNavigation;
            _characterSystemCommand = characterSystemCommand;
        }

        public InputPriority Priority => InputPriorities.Battle;

        public bool TryHandle(InputMessage message)
        {
            if (_appContext.Mode.CurrentValue != StageMode.Run) return false;

            if (_uiNavigation.IsRunning || _uiNavigation.HasOpenSystemUI) return false;

            switch (message.Type)
            {
                case InputMessageType.Submit:
                    return false;
                case InputMessageType.Cancel:
                    if (_uiNavigation.HasOpenStackUI) return false;

                    _sceneService.Load(SceneScopeIds.Lobby);
                    return true;
                case InputMessageType.ClickEnter:
                case InputMessageType.ClickExit:
                case InputMessageType.Point:
                    return false;
                case InputMessageType.Move:
                    if (_uiNavigation.HasOpenStackUI) return false;

                    return _characterSystemCommand.SetDirection(message.Value);
                case InputMessageType.Look:
                    return _characterSystemCommand.LookAt(_uiNavigation.HasOpenStackUI ? Vector2.zero : message.Value);

                case InputMessageType.LookCanceled:
                    if (_uiNavigation.HasOpenStackUI) return false;

                    return _characterSystemCommand.LookAt(Vector2.zero);
                case InputMessageType.Quick:
                case InputMessageType.QuickCanceled:
                    return false;
                case InputMessageType.Inventory:
                    if (!_uiNavigation.HasOpenStackUI)
                    {
                        _uiNavigation.ShowStackUIAsync(StackUIId.ItemContainer, new InventoryParams()).Forget();
                        _characterSystemCommand.Stop();
                        _uiNavigation.ApplyUILayer(UILayers.HideAll).Forget();
                        return true;
                    }

                    if (!_uiNavigation.IsTopOfStack(StackUIId.ItemContainer)) return false;

                    HideInventoryAsync().Forget();
                    return true;

                case InputMessageType.Interact:
                    if (_uiNavigation.HasOpenStackUI) return false;

                    _uiNavigation.ShowStackUIAsync(StackUIId.ItemContainer, new InventoryWithLootParams()).Forget();
                    _characterSystemCommand.Stop();
                    _uiNavigation.ApplyUILayer(UILayers.HideAll).Forget();
                    return true;

                case InputMessageType.Avoid:
                    if (_uiNavigation.HasOpenStackUI) return false;

                    return _characterSystemCommand.Avoid();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private async UniTaskVoid HideInventoryAsync()
        {
            await _uiNavigation.HideStackUIAsync(UIResult.Close);
            await _uiNavigation.ApplyUILayer(UILayers.Stage);
        }
    }
}