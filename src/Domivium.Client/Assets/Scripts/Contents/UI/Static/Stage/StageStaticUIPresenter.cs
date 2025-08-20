using System.Collections.Generic;
using Domivium.Client.Core.UI;
using Domivium.Client.Core.UI.Navigation;
using Domivium.Client.Core.UI.Presenter;

namespace Domivium.Client.Contents.UI.Static
{
    public class StageStaticUIPresenter : StaticUIPresenter<StageStaticUIView, IStageStaticUIMessage>, IStageStaticUIMessage
    {
        protected override HashSet<UILayer> Layer => UILayer.Set(UILayers.Stage);
        public override UIPriority Priority => UIPriorities.Stage;
        public StageStaticUIPresenter(StageStaticUIView view, IUINavigation navigation) : base(view, navigation) { }
    }
}