using System.Collections.Generic;
using Domivium.Client.Core.UI;
using Domivium.Client.Core.UI.Navigation;
using Domivium.Client.Core.UI.Presenter;

namespace Domivium.Client.Contents.UI.Static
{
    public class TitleStaticUIPresenter : StaticUIPresenter<TitleStaticUIView, ITitleStaticUIMessage>, ITitleStaticUIMessage
    {
        protected override HashSet<UILayer> Layer => UILayer.Set(UILayers.Title);
        public override UIPriority Priority => UIPriorities.Title;

        public TitleStaticUIPresenter(TitleStaticUIView view, IUINavigation navigation)
            : base(view, navigation) { }

        public void Login() { }
    }
}