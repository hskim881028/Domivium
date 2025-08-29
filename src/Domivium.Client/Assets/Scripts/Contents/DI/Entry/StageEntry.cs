using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Actors.Generated;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Input;
using Domivium.Client.Core.Message;
using Domivium.Client.Core.Utility;
using Domivium.Client.Data.Config;
using MessagePipe;

namespace Domivium.Client.Contents.DI.Entry
{
    public class StageEntry : Entry
    {
        private readonly IActorSpawner _actorSpawner;
        private readonly CancellationTokenSource _cts = new();

        public StageEntry(
            IActorSpawner actorSpawner,
            IInputComposition inputComposition,
            ISubscriber<InputMessage> subscriber)
        {
            _actorSpawner = actorSpawner;
        }

        protected override void OnStart()
        {
            RunAsync(new StageConfig
            {
                StageId = 1
            }).Forget();
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            _cts.Cancel();
            _cts.Dispose();
        }

        private async UniTask RunAsync(StageConfig cfg)
        {
            this.Log($"StageId: {cfg.StageId}");
            await _actorSpawner.SpawnAsync(ActorIds.Map);
            // todo: 유저 로드
            // 
            // stage map presenter, stage map actor

            // await _map.LoadAsync(cfg.StageId, ct);               // 맵/타일/네브/배경 등 준비
            // await _playerSpawner.SpawnAsync(cfg.PlayerSpawnCell, ct);
            // _pub.Publish(new StageEvents.StageStarted(cfg.StageId));
            // await _waves.RunWavesAsync(cfg, ct);                  // 웨이브 진행
            // _pub.Publish(new StageEvents.StageCleared(cfg.StageId));
        }

        private void Pause()
        {
            //todo: message pipe 연결
        }

        private void Resume()
        {
            //todo: message pipe 연결
        }

        private UniTask StopAsync()
        {
            _cts.Cancel();
            return UniTask.CompletedTask;
        }
    }
}