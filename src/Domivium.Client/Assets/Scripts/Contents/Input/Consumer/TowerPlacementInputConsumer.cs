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
        private readonly ITowerPlacementCommand _command;

        private bool IsTowerPlacementMode => _stageContext.Mode.CurrentValue == StageModes.TowerPlacement;

        public InputPriority Priority => InputPriorities.TowerPlacement;

        public TowerPlacementInputConsumer(StageContext stageContext, ITowerPlacementCommand command)
        {
            _stageContext = stageContext;
            _command = command;
        }

        public bool TryHandle(InputMessage message)
        {
            return message.Type switch
            {
                InputMessageType.Submit => false,
                InputMessageType.Cancel => IsTowerPlacementMode && _command.Hide(),
                InputMessageType.ClickEnter => false,
                InputMessageType.ClickExit => IsTowerPlacementMode && _command.Placement(message.Value),
                InputMessageType.Point => IsTowerPlacementMode && _command.Update(message.Value),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}