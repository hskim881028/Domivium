using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Core.Component;
using Domivium.Client.Core.UI.View;
using UnityEngine;

namespace Domivium.Client.Contents.UI.Static
{
    public class StageStaticUIView : StaticUIView<IStageStaticUIMessage>
    {
        [SerializeField] private DvmButton _enterLobbyButton;

        public override async UniTask InitializeAsync(CancellationToken token)
        {
            _enterLobbyButton.onClick.AddListener(Message.EnterLobby);
            await base.InitializeAsync(token);
        }
    }
}