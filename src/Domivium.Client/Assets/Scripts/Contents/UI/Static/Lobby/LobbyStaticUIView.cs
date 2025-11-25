using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Core.UI.View;
using UnityEngine;
using UnityEngine.UI;

namespace Domivium.Client.Contents.UI.Static
{
    public class LobbyStaticUIView : StaticUIView<ILobbyStaticUIMessage>
    {
        [SerializeField] private Button _nextButton;

        public override async UniTask<bool> InitializeAsync(CancellationToken token)
        {
            if (!await base.InitializeAsync(token)) return false;

            _nextButton.onClick.AddListener(Message.Next);
            return true;
        }
    }
}