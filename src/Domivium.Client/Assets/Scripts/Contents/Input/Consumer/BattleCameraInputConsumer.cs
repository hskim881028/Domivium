using System;
using Domivium.Client.Contents.Commands;
using Domivium.Client.Contents.Context;
using Domivium.Client.Core.Context;
using Domivium.Client.Core.Input;
using Domivium.Client.Core.Message;

namespace Domivium.Client.Contents.Input.Consumer
{
    public class BattleCameraInputConsumer : IInputConsumer
    {
        private readonly StageContext _stageContext;
        private readonly ICameraCommand _command;

        public BattleCameraInputConsumer(StageContext stageContext, ICameraCommand command)
        {
            _stageContext = stageContext;
            _command = command;
        }

        public InputPriority Priority => InputPriorities.Camera;

        private bool IsBattleMode => _stageContext.Mode.CurrentValue == StageModes.Battle;
        private bool IsMoveCameraMode => _stageContext.Mode.CurrentValue == StageModes.MoveCamera;
        private bool IsTerminated => _stageContext.Phase.CurrentValue == StagePhases.Cleared || _stageContext.Phase.CurrentValue == StagePhases.Failed;

        public bool TryHandle(InputMessage message)
        {
            if (IsTerminated) return false;

            switch (message.Type)
            {
                case InputMessageType.Submit:
                case InputMessageType.Cancel:
                case InputMessageType.Point:
                    return IsMoveCameraMode && _command.UpdatePosition(message.Value);
                case InputMessageType.ClickEnter:
                    return IsBattleMode && _command.MoveStarted(message.Value);
                case InputMessageType.ClickExit:
                    return IsMoveCameraMode && _command.MoveEnd(message.Value);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}