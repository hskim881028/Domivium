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
        [SerializeField] private RerollTowerItem _rerollTowerItem;
        [SerializeField] private CostText _soul;
        [SerializeField] private CostPairText _towerCount;

        public override async UniTask InitializeAsync(CancellationToken token)
        {
            _enterLobbyButton.onClick.AddListener(Message.EnterLobby);
            for (var i = 0; i < _towerItems.Count; i++)
            {
                var index = i;
                _towerItems[i].PointerDown = () => Message.SelectTower(index);
                _towerItems[i].PointerUp = () => Message.Cancel();
            }

            _rerollTowerItem.Button.onClick.AddListener(Message.Reroll);

            await base.InitializeAsync(token);
        }

        public void SetRerollCost(int cost)
        {
            _rerollTowerItem.SetCost(cost);
        }

        public void SetSoul(int soul)
        {
            _soul.Cost = soul;
        }

        public void SetTowerCount(int count)
        {
            _towerCount.Cost = count;
        }

        public void SetTowerLimit(int limit)
        {
            _towerCount.Limit = limit;
        }

        public void SetRerollDimmed(bool value)
        {
            _rerollTowerItem.SetDimmed(value);
        }

        public void SetDimmed(int slotIndex, bool value)
        {
            _towerItems[slotIndex].SetDimmed(value);
        }

        public void RefillTower(int slotIndex, int cost)
        {
            _towerItems[slotIndex].Refill(cost);
        }

        public void UserTower(int slotIndex)
        {
            _towerItems[slotIndex].Use();
        }
    }
}