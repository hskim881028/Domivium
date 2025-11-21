using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Core.UI.View;
using UnityEngine;
using UnityEngine.UI;

namespace Domivium.Client.Contents.UI.Static
{
    public class TitleStaticUIView : StaticUIView<ITitleStaticUIMessage>
    {
        [SerializeField] private Button _loginButton;

        public override async UniTask<bool> InitializeAsync(CancellationToken token)
        {
            if (!await base.InitializeAsync(token)) return false;

            _loginButton.onClick.AddListener(Message.Login);
            return true;
        }
    }
}