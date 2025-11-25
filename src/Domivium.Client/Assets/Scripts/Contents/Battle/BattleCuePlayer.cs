using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Actors.Contract;
using Domivium.Client.Contents.Actors.Generated;
using Domivium.Client.Contents.Audio.Generated;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Audio;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Message;
using MessagePipe;
using R3;

namespace Domivium.Client.Contents.Battle
{
    public class BattleCuePlayer : Disposable, IBattleCuePlayer
    {
        private readonly IAudioPlayer _audioPlayer;
        private readonly IActorSpawner _actorSpawner;

        public BattleCuePlayer(
            IAudioPlayer audioPlayer,
            IActorSpawner actorSpawner,
            ISubscriber<BattleCueMessage> subscriber)
        {
            _audioPlayer = audioPlayer;
            _actorSpawner = actorSpawner;
            subscriber.Subscribe(OnCueMessage).AddTo(ref DisposableBag);
        }

        private void OnCueMessage(BattleCueMessage message)
        {
            if (message.CueId == BattleCueIds.Attack)
            {
                _audioPlayer.PlaySFX(SFXAudioId.Attack);
            }

            if (message.CueId == BattleCueIds.Damaged)
            {
                _audioPlayer.PlaySFX(SFXAudioId.Damaged);
                var param = new DamageTextParams(message.Context.Position, message.Context.Value, 1.6f);
                _actorSpawner.SpawnAsync(ActorId.DamageText, param).Forget();
            }

            if (message.CueId == BattleCueIds.Healed)
            {
                var param = new HealTextParams(message.Context.Position, message.Context.Value, 1.6f);
                _actorSpawner.SpawnAsync(ActorId.HealText, param).Forget();
            }
        }
    }
}