using Domivium.Client.Core.UI.Navigation;
using Domivium.Client.Core.UI.View;

namespace Domivium.Client.Core.UI.Presenter
{
    public abstract class SystemUIPresenter<TView, TMessage> : UIPresenter<TView, TMessage>, ISystemUIPresenter
        where TView : SystemUIView<TMessage>
        where TMessage : IUIMessage
    {
        public abstract UIPriority Priority { get; }

        protected SystemUIPresenter(TView view, IUINavigation navigation) : base(view, navigation) { }
    }
}