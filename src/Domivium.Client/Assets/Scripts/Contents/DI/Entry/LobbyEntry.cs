using Domivium.Client.Core.Systems;
using Domivium.Client.Core.UI.Navigation;
using UnityEngine;
using VContainer.Unity;

namespace Domivium.Client.Contents.DI.Entry
{
    public class LobbyEntry : Entry, ITickable
    {
        private readonly ILobbySystemCommand _lobbySystemCommand;

        public LobbyEntry(
            IUINavigation uiNavigation,
            ILobbySystemCommand lobbySystemCommand
        ) : base(uiNavigation)
        {
            _lobbySystemCommand = lobbySystemCommand;
        }

        protected override void OnStart()
        {
            _lobbySystemCommand.RunAsync().Forget();
            // UINavigation.ApplyUILayer(UILayers.Lobby).Forget();
        }

        public void Tick()
        {
            _lobbySystemCommand.Tick(Time.deltaTime);
        }
    }
}