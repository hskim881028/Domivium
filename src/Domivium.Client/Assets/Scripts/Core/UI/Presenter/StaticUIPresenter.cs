using System.Collections.Generic;
using Domivium.Client.Core.Audio;
using Domivium.Client.Core.UI.Navigation;
using Domivium.Client.Core.UI.View;

namespace Domivium.Client.Core.UI.Presenter
{
    public abstract class StaticUIPresenter<TView, TMessage> : UIPresenter<TView, TMessage>, IStaticUIPresenter
        where TView : StaticUIView<TMessage>
        where TMessage : IUIMessage
    {
        protected abstract HashSet<UILayer> Layer { get; }
        public abstract UIPriority Priority { get; }

        protected StaticUIPresenter(TView view, IUINavigation navigation, IAudioController audioController)
            : base(view, navigation, audioController) { }

        public bool HasLayer(UILayer layer) => Layer.Contains(layer);
    }
}