using System.Collections.Generic;
using Domivium.Client.Contents.DI;
using Domivium.Client.Contents.Services;
using Domivium.Client.Core.UI;
using Domivium.Client.Core.UI.Navigation;
using Domivium.Client.Core.UI.Presenter;

namespace Domivium.Client.Contents.UI.Static
{
    public class StageStaticUIPresenter : StaticUIPresenter<StageStaticUIView, IStageStaticUIMessage>, IStageStaticUIMessage
    {
        private readonly SceneService _sceneService;
        protected override HashSet<UILayer> Layer => UILayer.Set(UILayers.Stage);
        public override UIPriority Priority => UIPriorities.Stage;

        public StageStaticUIPresenter(
            StageStaticUIView view,
            IUINavigation navigation,
            SceneService sceneService) : base(view, navigation)
        {
            _sceneService = sceneService;
        }

        public void EnterLobby()
        {
            _sceneService.Load(SceneScopeIds.Lobby);
        }
    }
}