using Domivium.Client.Contents.Audio.Generated;
using Domivium.Client.Core.Audio;

namespace Domivium.Client.Contents.DI.Entry
{
    public class LobbyEntry : Entry
    {
        public LobbyEntry(IAudioPlayer audioPlayer)
        {
            audioPlayer.PlayBGM(BGMAudioId.Lobby);
        }

        protected override void OnStart() { }
    }
}