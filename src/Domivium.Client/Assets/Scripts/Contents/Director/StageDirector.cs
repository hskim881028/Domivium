using System;
using Domivium.Client.Contents.Context;
using Domivium.Client.Core.Context;
using Domivium.Client.Core.Director;
using Domivium.Client.Core.Message;
using Domivium.Client.Core.Utility;
using MessagePipe;
using R3;

namespace Domivium.Client.Contents.Director
{
    public sealed class StageDirector : StageDirectorBase
    {
        public StageDirector(StageContext stageContext, ISubscriber<SceneMessage> subscriber) : base(stageContext)
        {
            subscriber.Subscribe(OnSceneMessage).AddTo(ref DisposableBag);
        }

        public override bool TrySetMode(StageMode mode)
        {
            if (Phase == StagePhases.Paused) return false;

            this.Log($"{mode.ToName()}");
            WriteMode(mode);
            return true;
        }

        public override bool TrySetPhase(StagePhase phase)
        {
            this.Log($"{phase.ToName()}");
            WritePhase(phase);
            return true;
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