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
        private readonly IAudioController _audioController;
        private readonly VfxService _vfxService;

        public BattleCuePlayer(
            IAudioController audioController,
            VfxService vfxService,
            ISubscriber<BattleCueMessage> subscriber)
        {
            _audioController = audioController;
            _vfxService = vfxService;
            subscriber.Subscribe(OnCueMessage).AddTo(ref DisposableBag);
        }

        private void OnCueMessage(BattleCueMessage message)
        {
            if (message.CueId == BattleCueIds.Slash)
            {
                _audioController.PlaySFX(SFXAudioId.Slash);
            }
            else if (message.CueId == BattleCueIds.Damaged)
            {
                _audioController.PlaySFX(SFXAudioId.Damaged);
            }
        }
    }
}