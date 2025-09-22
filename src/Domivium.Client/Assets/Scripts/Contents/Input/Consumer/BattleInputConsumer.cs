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
        private readonly IBattleCommand _command;

        public BattleInputConsumer(StageContext stageContext, IBattleCommand command)
        {
            _stageContext = stageContext;
            _command = command;
        }

        public InputPriority Priority => InputPriorities.Battle;

        private bool IsBattleMode => _stageContext.Mode.CurrentValue == StageModes.Battle;
        private bool IsTerminated => _stageContext.Phase.CurrentValue == StagePhases.Cleared || _stageContext.Phase.CurrentValue == StagePhases.Failed;

        public bool TryHandle(InputMessage message)
        {
            if (IsTerminated) return false;

            switch (message.Type)
            {
                case InputMessageType.Submit:
                case InputMessageType.Cancel:
                case InputMessageType.Point:
                    return IsBattleMode && _command.UpdateMoveTarget(message.Value);
                case InputMessageType.ClickEnter:
                    return IsBattleMode && _command.PickCharacter(message.Value);
                case InputMessageType.ClickExit:
                    return IsBattleMode && _command.SelectCharacter(message.Value);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}