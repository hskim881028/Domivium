using System;
using Domivium.Client.Contents.Actors.Generated;
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
        private readonly IActorFinder _actorFinder;

        public StageDirector(
            StageContext stageContext,
            IActorFinder actorFinder,
            ISubscriber<SceneMessage> sceneSubscriber) : base(stageContext)
        {
            _actorFinder = actorFinder;
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

        public override void Tick(float deltaTime)
        {
            if (Phase != StagePhases.RunningWave) return;

            if (_actorFinder.IsExistUnit(ActorIds.Nexus)) return;

            TrySetPhase(StagePhases.Failed);
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