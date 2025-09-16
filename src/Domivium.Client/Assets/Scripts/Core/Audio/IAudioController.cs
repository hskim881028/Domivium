namespace Domivium.Client.Core.Audio
{
    public interface IAudioController
    {
        public void SetMute(AudioParam param, bool mute);
        public void SetVolume(AudioParam param, float volume);
        public void PlayBGM(AudioId id);
        public void PlaySFX(AudioId id);
        public void PlayUI(AudioId id);
    }
}