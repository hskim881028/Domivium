using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Core.Component;
using Domivium.Client.Core.UI.View;
using UnityEngine;

namespace Domivium.Client.Contents.UI.Static
{
    public class TitleStaticUIView : StaticUIView<ITitleStaticUIMessage>
    {
        [SerializeField] private DvmButton _loginButton;

        public override async UniTask InitializeAsync(CancellationToken token)
        {
            _loginButton.onClick.AddListener(Message.Login);
            await base.InitializeAsync(token);
        }
    }
}