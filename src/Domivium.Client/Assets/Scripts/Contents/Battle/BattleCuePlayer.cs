using Domivium.Client.Contents.Audio.Generated;
using Domivium.Client.Contents.Services;
using Domivium.Client.Core.Audio;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Message;
using MessagePipe;
using R3;
using DisposableBag = R3.DisposableBag;

namespace Domivium.Client.Contents.Battle
{
    public class BattleCuePlayer : IBattleCuePlayer
    {
        private readonly IAudioController _audioController;
        private readonly VfxService _vfxService;
        private bool _isDisposed;
        private DisposableBag _disposable;

        public BattleCuePlayer(
            IAudioController audioController,
            VfxService vfxService,
            ISubscriber<BattleCueMessage> subscriber)
        {
            _audioController = audioController;
            _vfxService = vfxService;
            subscriber.Subscribe(OnCueMessage).AddTo(ref _disposable);
        }

        public void Dispose()
        {
            if (_isDisposed) return;

            _isDisposed = true;
            _disposable.Dispose();
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