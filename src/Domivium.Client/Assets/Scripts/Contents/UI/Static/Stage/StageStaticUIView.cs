using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Core.Component;
using Domivium.Client.Core.UI.View;
using UnityEngine;
using UnityEngine.UI;

namespace Domivium.Client.Contents.UI.Static
{
    public class StageStaticUIView : StaticUIView<IStageStaticUIMessage>
    {
        [SerializeField] private DvmButton _enterLobbyButton;
        [SerializeField] private Image[] _rightStickImages;
        [SerializeField] private FilledOnScreenButton _avoidButton;

        public override async UniTask InitializeAsync(CancellationToken token)
        {
            _enterLobbyButton.onClick.AddListener(Message.EnterLobby);
            await base.InitializeAsync(token);
        }

        public void SetAttackButton(bool canAttack)
        {
            var color = canAttack ? Constant.ActiveAttackButtonColor : Constant.IdleAttackButtonColor;
            foreach (var image in _rightStickImages)
            {
                image.color = color;
            }
        }

        public void SetAvoidButton(float cooldown)
        {
            _avoidButton.SetAsync(cooldown).Forget();
        }
    }
}