using Domivium.Client.Core.Audio;
using Domivium.Client.Core.UI.Contract;
using Domivium.Client.Core.UI.Navigation;
using Domivium.Client.Core.UI.View;

namespace Domivium.Client.Core.UI.Presenter
{
    public abstract class StackUIPresenter<TView, TMessage> : UIPresenter<TView, TMessage>, IStackUIPresenter
        where TView : StackUIView<TMessage>
        where TMessage : IUIMessage
    {
        protected StackUIPresenter(TView view, IUINavigation navigation, IAudioPlayer audioPlayer)
            : base(view, navigation, audioPlayer) { }

        protected void Close(UIResult result) { }
    }
}