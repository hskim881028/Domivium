using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Core.Component;
using Domivium.Client.Core.UI.View;
using UnityEngine;

namespace Domivium.Client.Contents.UI.Static
{
    public class LobbyStaticUIView : StaticUIView<ILobbyStaticUIMessage>
    {
        [SerializeField] private DvmButton _testButton;

        public override async UniTask<bool> InitializeAsync(CancellationToken token)
        {
            if (!await base.InitializeAsync(token)) return false;

            _testButton.onClick.AddListener(Message.Test);
            return true;
        }
    }
}