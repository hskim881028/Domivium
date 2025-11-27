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
    public sealed class BattleInputConsumer : InputConsumer
    {
        private readonly SceneService _sceneService;
        private readonly ICharacterSystemCommand _characterSystemCommand;

        public override InputPriority Priority => InputPriorities.Battle;

        public BattleInputConsumer(
            IAppContext appContext,
            IUINavigation uiNavigation,
            SceneService sceneService,
            ICharacterSystemCommand characterSystemCommand)
            : base(appContext, uiNavigation)
        {
            _sceneService = sceneService;
            _characterSystemCommand = characterSystemCommand;
        }

        public override bool TryHandle(InputMessage message)
        {
            if (AppContext.Mode.CurrentValue != StageMode.Run) return false;

            if (UINavigation.IsRunning || UINavigation.HasOpenSystemUI) return false;

            switch (message.Type)
            {
                case InputMessageType.Submit:
                    return false;
                case InputMessageType.Cancel:
                    if (UINavigation.HasOpenStackUI) return false;

                    if (AppContext.Scene.CurrentValue == SceneScopeIds.Lobby)
                    {
                        _sceneService.Load(SceneScopeIds.Stage);
                    }
                    else if (AppContext.Scene.CurrentValue == SceneScopeIds.Stage)
                    {
                        _sceneService.Load(SceneScopeIds.Lobby);
                    }

                    return true;
                case InputMessageType.ClickEnter:
                case InputMessageType.ClickExit:
                case InputMessageType.Point:
                    return false;
                case InputMessageType.Move:
                    if (UINavigation.HasOpenStackUI) return false;

                    return _characterSystemCommand.SetDirection(message.Value);
                case InputMessageType.Look:
                    return _characterSystemCommand.LookAt(UINavigation.HasOpenStackUI ? Vector2.zero : message.Value);

                case InputMessageType.LookCanceled:
                    if (UINavigation.HasOpenStackUI) return false;

                    return _characterSystemCommand.LookAt(Vector2.zero);
                case InputMessageType.Quick:
                case InputMessageType.QuickCanceled:
                    return false;
                case InputMessageType.Inventory:
                    if (!UINavigation.HasOpenStackUI)
                    {
                        UINavigation.ShowStackUIAsync(StackUIId.ItemContainer, new InventoryParams()).Forget();
                        _characterSystemCommand.Stop();
                        UINavigation.ApplyUILayer(UILayers.HideAll).Forget();
                        return true;
                    }

                    if (!UINavigation.IsTopOfStack(StackUIId.ItemContainer)) return false;

                    HideInventoryAsync().Forget();
                    return true;

                case InputMessageType.Interact:
                    if (UINavigation.HasOpenStackUI) return false;

                    UINavigation.ShowStackUIAsync(StackUIId.ItemContainer, new InventoryWithLootParams()).Forget();
                    _characterSystemCommand.Stop();
                    UINavigation.ApplyUILayer(UILayers.HideAll).Forget();
                    return true;

                case InputMessageType.Avoid:
                    if (UINavigation.HasOpenStackUI) return false;

                    return _characterSystemCommand.Avoid();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private async UniTaskVoid HideInventoryAsync()
        {
            await UINavigation.HideStackUIAsync(UIResult.Close);
            await ApplyUILayer();
        }
    }
}