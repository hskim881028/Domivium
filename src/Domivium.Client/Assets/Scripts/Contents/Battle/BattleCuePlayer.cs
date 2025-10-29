using Domivium.Client.Contents.Audio.Generated;
using Domivium.Client.Contents.Services;
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
        private readonly VfxService _vfxService;

        public BattleCuePlayer(
            IAudioPlayer audioPlayer,
            VfxService vfxService,
            ISubscriber<BattleCueMessage> subscriber)
        {
            _audioPlayer = audioPlayer;
            _vfxService = vfxService;
            subscriber.Subscribe(OnCueMessage).AddTo(ref DisposableBag);
        }

        private void OnCueMessage(BattleCueMessage message)
        {
            if (message.CueId == BattleCueIds.Attack)
            {
                _audioPlayer.PlaySFX(SFXAudioId.Attack);
            }
            else if (message.CueId == BattleCueIds.Damaged)
            {
                _audioPlayer.PlaySFX(SFXAudioId.Damaged);
            }

            _vfxService.Spawn(message.CueId, message.Context);
        }
    }
}