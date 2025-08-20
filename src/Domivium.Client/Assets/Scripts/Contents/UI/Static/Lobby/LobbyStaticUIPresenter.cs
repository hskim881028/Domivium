using System.Collections.Generic;
using Domivium.Client.Core.UI;
using Domivium.Client.Core.UI.Navigation;
using Domivium.Client.Core.UI.Presenter;

namespace Domivium.Client.Contents.UI.Static
{
    public class LobbyStaticUIPresenter : StaticUIPresenter<LobbyStaticUIView, ILobbyStaticUIMessage>, ILobbyStaticUIMessage
    {
        protected override HashSet<UILayer> Layer => UILayer.Set(UILayers.Lobby);
        public override UIPriority Priority => UIPriorities.Lobby;
        public LobbyStaticUIPresenter(LobbyStaticUIView view, IUINavigation navigation) : base(view, navigation) { }
    }
}