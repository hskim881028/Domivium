using Domivium.Client.Core.Audio;
using Domivium.Client.Core.UI.Navigation;
using Domivium.Client.Core.UI.Presenter;

namespace Domivium.Client.Contents.UI.Stack
{
    public class LoginPopupStackUIPresenter : StackUIPresenter<LoginPopupStackUIView, ILoginPopupStackUIMessage>, ILoginPopupStackUIMessage
    {
        public LoginPopupStackUIPresenter(
            LoginPopupStackUIView view,
            IUINavigation navigation,
            IAudioPlayer audioPlayer)
            : base(view, navigation, audioPlayer) { }
    }
}