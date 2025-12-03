using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Services;
using Domivium.Client.Core.Container;
using Domivium.Client.Core.UI.Navigation;

namespace Domivium.Client.Contents.DI.Entry
{
    public class BootstrapEntry : Entry
    {
        private readonly IUserContainer _userContainer;
        private readonly SceneService _sceneService;

        public BootstrapEntry(
            IUINavigation uiNavigation,
            IUserContainer userContainer,
            SceneService sceneService) : base(uiNavigation)
        {
            _userContainer = userContainer;
            _sceneService = sceneService;
        }

        protected override void OnStart()
        {
            InitializeAsync().Forget();
        }

        private async UniTaskVoid InitializeAsync()
        {
            await _userContainer.InitializeUserAsync(1, 1);
            await _userContainer.InitializeItemAsync();
            _sceneService.Load(SceneScopeIds.Lobby);
        }
    }
}