using System;
using Domivium.Client.Contents.Context;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Context;
using Domivium.Client.Core.Director;
using Domivium.Client.Core.Message;
using MessagePipe;
using R3;

namespace Domivium.Client.Contents.Director
{
    public sealed class StageDirector : StageDirectorBase
    {
        private readonly InputEventSystem _inputEventSystem;

        public StageDirector(
            StageContext stageContext,
            InputEventSystem inputEventSystem,
            ISubscriber<SceneMessage> sceneSubscriber) : base(stageContext)
        {
            _inputEventSystem = inputEventSystem;
            stageContext.Mode.Subscribe(OnChangeMode).AddTo(ref DisposableBag);
            sceneSubscriber.Subscribe(OnSceneMessage).AddTo(ref DisposableBag);
        }

        public override bool TrySetMode(StageMode mode)
        {
            if (Phase == StagePhases.Paused) return false;

            WriteMode(mode);
            return true;
        }

        public override bool TrySetPhase(StagePhase phase)
        {
            this.Log($"{phase.ToName()}");
            WritePhase(phase);
            return true;
        }

        private void OnChangeMode(StageMode mode)
        {
            if (mode == StageModes.TowerPlacement ||
                mode == StageModes.MoveCharacter ||
                mode == StageModes.MoveCamera)
            {
                _inputEventSystem.BlockUI = true;
            }
            else
            {
                _inputEventSystem.BlockUI = false;
            }
        }

        private void OnSceneMessage(SceneMessage message)
        {
            switch (message.Type)
            {
                case SceneMessageType.Unload:
                case SceneMessageType.Load:
                    WriteMode(StageMode.Idle);
                    WritePhase(StagePhase.Idle);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}