using System.Collections.Generic;
using Domivium.Client.Core.Audio;
using Domivium.Client.Core.UI;
using Domivium.Client.Core.UI.Navigation;
using Domivium.Client.Core.UI.Presenter;

namespace Domivium.Client.Contents.UI.Static
{
    public class BootstrapStaticUIPresenter : StaticUIPresenter<BootstrapStaticUIView, IBootstrapStaticUIMessage>, IBootstrapStaticUIMessage
    {
        protected override HashSet<UILayer> Layer { get; }
        public override UIPriority Priority { get; }

        public BootstrapStaticUIPresenter(
            BootstrapStaticUIView view,
            IUINavigation navigation,
            IAudioPlayer audioController)
            : base(view, navigation, audioController) { }
    }
}