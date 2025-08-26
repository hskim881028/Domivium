using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Services;
using Domivium.Client.Core.UI;
using Domivium.Client.Core.UI.Navigation;
using Domivium.Client.Core.UI.Presenter;
using Domivium.Client.Core.Utility;
using Domivium.Shared.Request;
using Domivium.Shared.Services;

namespace Domivium.Client.Contents.UI.Static
{
    public class LobbyStaticUIPresenter : StaticUIPresenter<LobbyStaticUIView, ILobbyStaticUIMessage>, ILobbyStaticUIMessage
    {
        private readonly NetworkService _networkService;

        protected override HashSet<UILayer> Layer => UILayer.Set(UILayers.Lobby);
        public override UIPriority Priority => UIPriorities.Lobby;

        public LobbyStaticUIPresenter(
            LobbyStaticUIView view,
            IUINavigation navigation,
            NetworkService networkService) : base(view, navigation)
        {
            _networkService = networkService;
        }

        public void Test()
        {
            GetCharacterAsync().Forget();
        }

        private async UniTaskVoid GetCharacterAsync()
        {
            var characterService = _networkService.CreateService<ICharacterService>();
            var response = await characterService.Value.GetCharactersAsync(new GetCharactersRequest());
            if (!_networkService.HandleResponse(response)) return;

            this.Log($"{response.Message}");
        }
    }
}