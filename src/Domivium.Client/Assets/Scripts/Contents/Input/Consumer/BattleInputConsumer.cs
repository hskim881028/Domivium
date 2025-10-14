using System;
using Domivium.Client.Contents.Commands;
using Domivium.Client.Contents.Context;
using Domivium.Client.Core.Context;
using Domivium.Client.Core.Input;
using Domivium.Client.Core.Message;

namespace Domivium.Client.Contents.Input.Consumer
{
    public sealed class BattleInputConsumer : IInputConsumer
    {
        private readonly StageContext _stageContext;
        private readonly IBattleUserCommand _userCommand;

        public BattleInputConsumer(StageContext stageContext, IBattleUserCommand userCommand)
        {
            _stageContext = stageContext;
            _userCommand = userCommand;
        }

        public InputPriority Priority => InputPriorities.Battle;

        private bool IsBattleMode => _stageContext.Mode.CurrentValue == StageModes.Battle;
        private bool IsMoveCharacterMode => _stageContext.Mode.CurrentValue == StageModes.MoveCharacter;
        private bool IsTerminated => _stageContext.Phase.CurrentValue == StagePhases.Cleared || _stageContext.Phase.CurrentValue == StagePhases.Failed;

        public bool TryHandle(InputMessage message)
        {
            if (IsTerminated) return false;

            switch (message.Type)
            {
                case InputMessageType.Submit:
                case InputMessageType.Cancel:
                case InputMessageType.Point:
                    return IsMoveCharacterMode && _userCommand.UpdateMoveTarget(message.Value);
                case InputMessageType.ClickEnter:
                    return IsBattleMode && _userCommand.PickCharacter(message.Value);
                case InputMessageType.ClickExit:
                    return IsMoveCharacterMode && _userCommand.SelectCharacter(message.Value);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}