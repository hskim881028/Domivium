using System;
using Domivium.Client.Contents.Actors.Generated;
using Domivium.Client.Contents.Context;
using Domivium.Client.Contents.ReadModels;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Context;
using Domivium.Client.Core.Director;
using Domivium.Client.Core.Message;
using Domivium.Client.Core.State;
using MessagePipe;
using R3;

namespace Domivium.Client.Contents.Director
{
    public sealed class StageDirector : StageDirectorBase
    {
        private readonly IBattleService _battleService;

        public StageDirector(
            StageContext stageContext,
            IBattleService battleService,
            ISubscriber<SceneMessage> sceneSubscriber,
            ISubscriber<ActorStateMessage> actorStateSubscriber) : base(stageContext)
        {
            _battleService = battleService;
            sceneSubscriber.Subscribe(OnSceneMessage).AddTo(ref DisposableBag);
            actorStateSubscriber.Subscribe(OnActorStateMessage).AddTo(ref DisposableBag);
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

        private void OnActorStateMessage(ActorStateMessage message)
        {
            if (Phase != StagePhases.RunningWave) return;

            if (message.ActorId == ActorIds.Nexus &&
                message.Tag == StateTag.Die &&
                !_battleService.IsExistUnit(ActorIds.Nexus))
            {
                TrySetPhase(StagePhases.Failed);
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