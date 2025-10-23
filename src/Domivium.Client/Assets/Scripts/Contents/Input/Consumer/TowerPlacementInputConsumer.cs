using System;
using Domivium.Client.Contents.Commands;
using Domivium.Client.Contents.Context;
using Domivium.Client.Core.Context;
using Domivium.Client.Core.Input;
using Domivium.Client.Core.Message;

namespace Domivium.Client.Contents.Input.Consumer
{
    public sealed class TowerPlacementInputConsumer : IInputConsumer
    {
        private readonly StageContext _stageContext;
        private readonly ITowerPlacementCommand _towerPlacementCommand;

        public InputPriority Priority => InputPriorities.TowerPlacement;
        private bool IsTowerPlacementMode => _stageContext.Mode.CurrentValue == StageModes.TowerPlacement;
        private bool IsTerminated => _stageContext.Phase.CurrentValue == StagePhases.Cleared || _stageContext.Phase.CurrentValue == StagePhases.Failed;

        public TowerPlacementInputConsumer(StageContext stageContext, ITowerPlacementCommand towerPlacementCommand)
        {
            _stageContext = stageContext;
            _towerPlacementCommand = towerPlacementCommand;
        }

        public bool TryHandle(InputMessage message)
        {
            if (IsTerminated) return false;

            switch (message.Type)
            {
                case InputMessageType.Submit:
                    return false;
                case InputMessageType.Cancel:
                    return IsTowerPlacementMode && _towerPlacementCommand.Hide();
                case InputMessageType.ClickEnter:
                    return false;
                case InputMessageType.ClickExit:
                    if (!IsTowerPlacementMode) return false;

                    _towerPlacementCommand.Placement(message.Value);
                    return true;
                case InputMessageType.Point:
                    return IsTowerPlacementMode && _towerPlacementCommand.Update(message.Value);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}