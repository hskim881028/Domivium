using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Services;
using Domivium.Client.Core.Scene;
using Domivium.Client.Core.UI;
using Domivium.Client.Core.UI.Navigation;
using Domivium.Client.Core.UI.Presenter;
using Domivium.Client.Core.Utility;
using Domivium.Shared.Services;

namespace Domivium.Client.Contents.UI.Static
{
    public class TitleStaticUIPresenter : StaticUIPresenter<TitleStaticUIView, ITitleStaticUIMessage>, ITitleStaticUIMessage
    {
        private readonly NetworkService _networkService;
        private readonly SceneService _sceneService;
        protected override HashSet<UILayer> Layer => UILayer.Set(UILayers.Title);
        public override UIPriority Priority => UIPriorities.Title;

        public TitleStaticUIPresenter(
            TitleStaticUIView view,
            IUINavigation navigation,
            NetworkService networkService,
            SceneService sceneService)
            : base(view, navigation)
        {
            _networkService = networkService;
            _sceneService = sceneService;
        }

        public void Login()
        {
            LoginAsync().Forget();
        }

        private async UniTaskVoid LoginAsync()
        {
            var loginService = _networkService.CreateService<ILoginService>();
            var response = await loginService.Value.Login();
            if (!_networkService.HandleResponse(response)) return;

            this.Log($"[StatusCode]: {response.StatusCode}");
            _sceneService.Load(SceneScopeId.Lobby);
            this.Log();
        }
    }
}