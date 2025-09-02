using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Components;
using Domivium.Client.Core.Component;
using Domivium.Client.Core.UI.View;
using UnityEngine;

namespace Domivium.Client.Contents.UI.Static
{
    public class StageStaticUIView : StaticUIView<IStageStaticUIMessage>
    {
        [SerializeField] private DvmButton _enterLobbyButton;
        [SerializeField] private List<SelectTowerItem> _towerItems;

        public override async UniTask InitializeAsync(CancellationToken token)
        {
            _enterLobbyButton.onClick.AddListener(Message.EnterLobby);
            for (var i = 0; i < _towerItems.Count; i++)
            {
                var index = i;
                _towerItems[i].PointerDown = () => Message.SelectTower(index);
                _towerItems[i].PointerUp = () => Message.Cancel();
            }

            await base.InitializeAsync(token);
        }
    }
}