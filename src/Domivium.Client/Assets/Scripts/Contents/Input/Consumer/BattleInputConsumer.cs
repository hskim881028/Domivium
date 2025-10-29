using System;
using Domivium.Client.Contents.System.Command;
using Domivium.Client.Core.Context;
using Domivium.Client.Core.Input;
using Domivium.Client.Core.Message;

namespace Domivium.Client.Contents.Input.Consumer
{
    public sealed class BattleInputConsumer : IInputConsumer
    {
        private readonly StageContext _stageContext;
        private readonly ICharacterSystemCommand _characterSystemCommand;

        public BattleInputConsumer(StageContext stageContext, ICharacterSystemCommand characterSystemCommand)
        {
            _stageContext = stageContext;
            _characterSystemCommand = characterSystemCommand;
        }

        public InputPriority Priority => InputPriorities.Battle;

        private bool IsStageRunning => _stageContext.Mode.CurrentValue == StageMode.Run;

        public bool TryHandle(InputMessage message)
        {
            if (!IsStageRunning) return false;

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
                    return _characterSystemCommand.Firing();
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