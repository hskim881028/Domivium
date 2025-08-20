using Domivium.Client.Core.UI.Navigation;
using Domivium.Client.Core.UI.Presenter;

namespace Domivium.Client.Contents.UI.Stack
{
    public class LoginStackUIPresenter : StackUIPresenter<LoginStackUIView, ILoginStackUIMessage>, ILoginStackUIMessage
    {
        public LoginStackUIPresenter(LoginStackUIView view, IUINavigation navigation) : base(view, navigation) { }
    }
}