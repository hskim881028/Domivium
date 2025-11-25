using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Audio.Generated;
using Domivium.Client.Contents.UI;
using Domivium.Client.Core.Audio;
using Domivium.Client.Core.UI.Navigation;

namespace Domivium.Client.Contents.DI.Entry
{
    public class LobbyEntry : Entry
    {
        public LobbyEntry(
            IUINavigation uiNavigation,
            IAudioPlayer audioPlayer) : base(uiNavigation)
        {
            audioPlayer.PlayBGM(BGMAudioId.Lobby);
        }

        protected override void OnStart()
        {
            UINavigation.ApplyUILayer(UILayers.Lobby).Forget();
        }
    }
}