using System;
using Domivium.Client.Core.Input;
using Domivium.Client.Core.Message;
using Domivium.Client.Core.Systems;

namespace Domivium.Client.Contents.Input.Consumer
{
    public sealed class BattleInputConsumer : IInputConsumer
    {
        private readonly IStageSystem _stageSystem;
        private readonly ICharacterSystemCommand _characterSystemCommand;

        public BattleInputConsumer(IStageSystem stageSystem, ICharacterSystemCommand characterSystemCommand)
        {
            _stageSystem = stageSystem;
            _characterSystemCommand = characterSystemCommand;
        }

        public InputPriority Priority => InputPriorities.Battle;

        public bool TryHandle(InputMessage message)
        {
            if (_stageSystem.Mode.CurrentValue != StageMode.Run) return false;

            switch (message.Type)
            {
                case InputMessageType.Submit:
                case InputMessageType.Cancel:
                case InputMessageType.ClickEnter:
                case InputMessageType.ClickExit:
                case InputMessageType.Point:
                    return false;
                case InputMessageType.Move:
                    return _characterSystemCommand.SetDirection(message.Value);
                case InputMessageType.Look:
                    return _characterSystemCommand.LookAt(message.Value);
                case InputMessageType.LookCanceled:
                    return _characterSystemCommand.Reload();
                case InputMessageType.Quick:
                case InputMessageType.QuickCanceled:
                case InputMessageType.Inventory:
                case InputMessageType.Interact:
                    return false;
                case InputMessageType.Avoid:
                    return _characterSystemCommand.Avoid();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}